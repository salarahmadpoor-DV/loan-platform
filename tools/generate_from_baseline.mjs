import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");
const ENT = path.join(ROOT, "Matchi.Domain", "Entities");
const CFG = path.join(ROOT, "Matchi.Infrastructure", "Persistence", "Configurations");
const TSV = path.join(ROOT, "tools", "schema_columns.tsv");
const PY = fs.readFileSync(path.join(__dirname, "generate_from_baseline.py"), "utf8");

function pyToJs(src) {
  let out = "";
  let inStr = false;
  for (let i = 0; i < src.length; i++) {
    const ch = src[i];
    if (inStr) {
      out += ch;
      if (ch === '"' && src[i - 1] !== "\\") inStr = false;
      continue;
    }
    if (ch === '"') {
      inStr = true;
      out += ch;
      continue;
    }
    if (ch === "(") out += "[";
    else if (ch === ")") out += "]";
    else out += ch;
  }
  return out.replace(/\bTrue\b/g, "true").replace(/\bFalse\b/g, "false").replace(/\bNone\b/g, "null");
}

function grab(name, kind) {
  const startToken = `${name} = ${kind}`;
  const start = PY.indexOf(startToken);
  if (start < 0) throw new Error(`missing ${name}`);
  const open = kind;
  const close = kind === "{" ? "}" : "]";
  let i = start + startToken.length - 1;
  let depth = 0;
  for (; i < PY.length; i++) {
    const ch = PY[i];
    if (ch === open) depth++;
    else if (ch === close) {
      depth--;
      if (depth === 0) {
        let src = PY.slice(start + `${name} = `.length, i + 1);
        src = pyToJs(src);
        return Function(`"use strict"; return (${src});`)();
      }
    }
  }
  throw new Error(`unclosed ${name}`);
}

const TABLE_CLASS = grab("TABLE_CLASS", "{");
const FKS = grab("FKS", "[");
const INDEXES = grab("INDEXES", "[");
const CHECKS = grab("CHECKS", "[");
const PKS = Object.fromEntries(Object.keys(TABLE_CLASS).map((t) => [t, ["Id"]]));
PKS.UserRoles = ["UserId", "RoleId"];
PKS.RolePermissions = ["RoleId", "PermissionId"];
const PUBLIC_SETTER = new Set(["Role", "Permission", "UserRole", "RolePermission"]);

function parseColumns() {
  const tables = {};
  for (const line of fs.readFileSync(TSV, "utf8").split(/\r?\n/)) {
    if (!line.trim()) continue;
    const parts = line.split("\t");
    const [table, col, ord, dtype, maxlen, prec, scale, nullable, def = ""] = parts;
    (tables[table] ??= []).push({
      name: col,
      ord: Number(ord),
      dtype,
      maxlen,
      prec,
      scale,
      nullable: nullable === "YES",
      default: def,
    });
  }
  return tables;
}

function csharpType(col) {
  const mapping = {
    bigint: "long",
    int: "int",
    tinyint: "byte",
    bit: "bool",
    decimal: "decimal",
    datetime2: "DateTime",
    date: "DateOnly",
    time: "TimeSpan",
    nvarchar: "string",
  };
  const t = mapping[col.dtype];
  if (t === "string") return col.nullable ? "string?" : "string";
  return col.nullable ? `${t}?` : t;
}

function defaultCsharp(col) {
  let d = col.default;
  if (!d) return null;
  d = d.trim();
  if (col.dtype === "bit") {
    if (d === "((0))" || d === "(0)") return "false";
    if (d === "((1))" || d === "(1)") return "true";
  }
  if (d === "((0))" || d === "(0)") return col.dtype === "decimal" ? "0m" : "0";
  if (d === "((1))" || d === "(1)") return col.dtype === "decimal" ? "1m" : "1";
  if (d === "(N'Active')") return '"Active"';
  if (d === "(N'Open')") return '"Open"';
  if (d === "(N'Pending')") return '"Pending"';
  if (d === "(N'Assigned')") return '"Assigned"';
  return null;
}

function baseClass(cols, className) {
  const names = new Set(cols.map((c) => c.name));
  if (className === "UserRole" || className === "RolePermission") return null;
  if (names.has("IsDeleted")) return "AuditableEntity";
  if (names.has("CreateDate") && names.has("UpdateDate")) return "TimestampedEntity";
  return "Entity";
}

function inheritedProps(base) {
  if (base === "AuditableEntity") return new Set(["Id", "CreateDate", "UpdateDate", "IsDeleted"]);
  if (base === "TimestampedEntity") return new Set(["Id", "CreateDate", "UpdateDate"]);
  if (base === "Entity") return new Set(["Id"]);
  return new Set();
}

function write(file, content) {
  fs.mkdirSync(path.dirname(file), { recursive: true });
  fs.writeFileSync(file, content.replace(/\r\n/g, "\n"), "utf8");
}

function buildCtor(className, props) {
  const special = {
    User: [
      "    public User(string mobile)",
      "    {",
      "        Mobile = mobile;",
      "        IsMobileVerified = false;",
      "    }",
      "",
      "    public void VerifyMobile()",
      "    {",
      "        IsMobileVerified = true;",
      "        SetUpdated();",
      "    }",
      "",
      "    public void UpdateProfile(string? name)",
      "    {",
      "        Name = name;",
      "        SetUpdated();",
      "    }",
    ],
    Provider: [
      "    public Provider(long userId, string name, string mobile, decimal? lat = null, decimal? lng = null, string? description = null)",
      "    {",
      "        UserId = userId;",
      "        Name = name;",
      "        Mobile = mobile;",
      "        Lat = lat;",
      "        Lng = lng;",
      "        Description = description;",
      "        Rating = 0m;",
      "        ReviewCount = 0;",
      "        CompletedJobCount = 0;",
      '        Status = "Active";',
      "    }",
    ],
    Business: [
      "    public Business(long ownerUserId, string name, string? address = null, decimal? lat = null, decimal? lng = null)",
      "    {",
      "        OwnerUserId = ownerUserId;",
      "        Name = name;",
      "        Address = address;",
      "        Lat = lat;",
      "        Lng = lng;",
      "        Rating = 0m;",
      "        ReviewCount = 0;",
      "        CompletedJobCount = 0;",
      '        Status = "Active";',
      "    }",
    ],
    Customer: ["    public Customer(long userId)", "    {", "        UserId = userId;", "    }"],
    Service: [
      "    public Service(string name, long categoryId, string? slug = null, string? description = null, int displayOrder = 0)",
      "    {",
      "        Name = name;",
      "        CategoryId = categoryId;",
      "        Slug = slug;",
      "        Description = description;",
      "        DisplayOrder = displayOrder;",
      "        IsActive = true;",
      "    }",
    ],
    ServiceCategory: [
      "    public ServiceCategory(string name, string slug, int displayOrder = 0)",
      "    {",
      "        Name = name;",
      "        Slug = slug;",
      "        DisplayOrder = displayOrder;",
      "        IsActive = true;",
      "    }",
    ],
    ProviderService: [
      "    public ProviderService(long providerId, long serviceId)",
      "    {",
      "        ProviderId = providerId;",
      "        ServiceId = serviceId;",
      "        IsActive = true;",
      "    }",
    ],
    BusinessProvider: [
      "    public BusinessProvider(long businessId, long providerId, string role = \"Member\")",
      "    {",
      "        BusinessId = businessId;",
      "        ProviderId = providerId;",
      "        Role = role;",
      '        Status = "Active";',
      "        JoinedAt = DateTime.UtcNow;",
      "    }",
    ],
    Request: [
      "    public Request(long customerId, string requestType, string title, string? description = null)",
      "    {",
      "        CustomerId = customerId;",
      "        RequestType = requestType;",
      "        Title = title;",
      "        Description = description;",
      '        Status = "Open";',
      "    }",
    ],
    RequestLocation: [
      "    public RequestLocation(long requestId, decimal? lat = null, decimal? lng = null, string? address = null)",
      "    {",
      "        RequestId = requestId;",
      "        Lat = lat;",
      "        Lng = lng;",
      "        Address = address;",
      "    }",
    ],
    RequestServiceAttribute: [
      "    public RequestServiceAttribute(long requestServiceId, long serviceAttributeId, string? value = null)",
      "    {",
      "        RequestServiceId = requestServiceId;",
      "        ServiceAttributeId = serviceAttributeId;",
      "        Value = value;",
      "    }",
    ],
  };
  if (special[className]) return special[className];

  const req = props.filter((c) => {
    if (c.nullable) return false;
    if (defaultCsharp(c) != null) return false;
    if (["datetime2", "date", "time"].includes(c.dtype) && c.default) return false;
    if (["CreateDate", "UpdateDate", "IsDeleted", "Id"].includes(c.name)) return false;
    return true;
  });
  if (!req.length) return null;
  const params = req.map((c) => `${csharpType(c)} ${c.name[0].toLowerCase()}${c.name.slice(1)}`);
  const assigns = req.map((c) => `        ${c.name} = ${c.name[0].toLowerCase()}${c.name.slice(1)};`);
  return [`    public ${className}(${params.join(", ")})`, "    {", ...assigns, "    }"];
}

function emitEntity(className, table, cols, navsRef, navsCol) {
  const base = baseClass(cols, className);
  const skip = inheritedProps(base);
  const pub = PUBLIC_SETTER.has(className);
  const setter = pub ? "set" : "private set";
  const lines = ["using Matchi.Domain.Common;", "", "namespace Matchi.Domain.Entities;", ""];
  lines.push(base ? `public class ${className} : ${base}` : `public class ${className}`);
  lines.push("{");
  const props = cols.filter((c) => !skip.has(c.name));
  for (const c of props) {
    const t = csharpType(c);
    let init = "";
    if (t === "string") init = " = null!";
    const d = defaultCsharp(c);
    if (d != null && t !== "string" && t !== "string?") {
      if (t === "bool") init = ` = ${d}`;
      else if (["int", "long", "byte", "decimal"].includes(t) && c.name !== "Id") init = ` = ${d}`;
    }
    if (t === "string" && d && d.startsWith('"')) init = ` = ${d}`;
    if (init)
      lines.push(`    public ${t} ${c.name} { get; ${setter}; }${init};`);
    else
      lines.push(`    public ${t} ${c.name} { get; ${setter}; }`);
    lines.push("");
  }
  for (const [nav, typ, required] of navsRef) {
    lines.push(required ? `    public ${typ} ${nav} { get; ${setter}; } = null!;` : `    public ${typ}? ${nav} { get; ${setter}; }`);
    lines.push("");
  }
  for (const [nav, typ] of navsCol) {
    lines.push(`    public ICollection<${typ}> ${nav} { get; ${setter}; } = new List<${typ}>();`);
    lines.push("");
  }
  if (!pub) {
    lines.push(`    private ${className}()`, "    {", "    }", "");
    const ctor = buildCtor(className, props);
    if (ctor) {
      lines.push(...ctor);
      lines.push("");
    }
  }
  while (lines.at(-1) === "") lines.pop();
  lines.push("}", "");
  write(path.join(ENT, `${className}.cs`), lines.join("\n"));
}

function indexExpr(cols) {
  if (cols.length === 1) return `x => x.${cols[0]}`;
  return `x => new { ${cols.map((c) => `x.${c}`).join(", ")} }`;
}

function emitConfig(className, table, cols) {
  const base = baseClass(cols, className);
  const checks = CHECKS.filter((x) => x[0] === table).map((x) => [x[1], x[2]]);
  const lines = [
    "using Matchi.Domain.Entities;",
    "using Microsoft.EntityFrameworkCore;",
    "using Microsoft.EntityFrameworkCore.Metadata.Builders;",
    "",
    "namespace Matchi.Infrastructure.Persistence.Configurations;",
    "",
    `public class ${className}Configuration : IEntityTypeConfiguration<${className}>`,
    "{",
    `    public void Configure(EntityTypeBuilder<${className}> builder)`,
    "    {",
  ];
  if (checks.length) {
    lines.push(`        builder.ToTable("${table}", t =>`, "        {");
    for (const [n, sql] of checks) lines.push(`            t.HasCheckConstraint("${n}", "${sql.replace(/"/g, '\\"')}");`);
    lines.push("        });");
  } else lines.push(`        builder.ToTable("${table}");`);
  lines.push("");
  const pk = PKS[table];
  if (pk.length === 1 && pk[0] === "Id") {
    lines.push(`        builder.HasKey(x => x.Id).HasName("PK_${table}");`);
    lines.push("        builder.Property(x => x.Id).ValueGeneratedOnAdd();");
  } else {
    lines.push(`        builder.HasKey(x => new { ${pk.map((c) => `x.${c}`).join(", ")} }).HasName("PK_${table}");`);
  }
  lines.push("");
  const skip = inheritedProps(base);
  if (base === "AuditableEntity") lines.push("        builder.ConfigureAuditable();", "");
  else if (base === "TimestampedEntity") lines.push("        builder.ConfigureTimestamped();", "");
  else if (cols.some((c) => c.name === "CreateDate")) {
    lines.push("        builder.Property(x => x.CreateDate)", "            .IsRequired()", '            .HasDefaultValueSql("sysutcdatetime()");', "");
    skip.add("CreateDate");
  }
  for (const c of cols) {
    if (skip.has(c.name) || c.name === "Id") continue;
    const chain = [];
    if (!c.nullable) chain.push("IsRequired()");
    if (c.dtype === "nvarchar") {
      if (c.maxlen === "-1") chain.push('HasColumnType("nvarchar(max)")');
      else if (c.maxlen) chain.push(`HasMaxLength(${c.maxlen})`);
    }
    if (c.dtype === "decimal") chain.push(`HasPrecision(${c.prec}, ${c.scale})`);
    if (c.dtype === "tinyint") chain.push('HasColumnType("tinyint")');
    if (c.dtype === "date") chain.push('HasColumnType("date")');
    if (c.dtype === "time") chain.push('HasColumnType("time")');
    if (c.default) {
      const dc = defaultCsharp(c);
      if (dc != null) {
        if (dc === "true" || dc === "false" || dc.endsWith("m") || /^-?\d+$/.test(dc) || dc.startsWith('"')) chain.push(`HasDefaultValue(${dc})`);
      } else if (c.default.toLowerCase().includes("sysutcdatetime")) chain.push('HasDefaultValueSql("sysutcdatetime()")');
    }
    if (chain.length) {
      lines.push(`        builder.Property(x => x.${c.name})`);
      chain.forEach((ch, i) => lines.push(`            .${ch}${i === chain.length - 1 ? ";" : ""}`));
    }
    lines.push("");
  }
  for (const [dep, fk, , depNav, prinNav, one, cname] of FKS) {
    if (dep !== className) continue;
    lines.push(`        builder.HasOne(x => x.${depNav})`);
    if (one) {
      lines.push(`            .WithOne(x => x.${prinNav})`);
      lines.push(`            .HasForeignKey<${className}>(x => x.${fk})`);
    } else {
      lines.push(`            .WithMany(x => x.${prinNav})`);
      lines.push(`            .HasForeignKey(x => x.${fk})`);
    }
    lines.push(`            .HasConstraintName("${cname}")`);
    lines.push("            .OnDelete(DeleteBehavior.NoAction);");
    lines.push("");
  }
  for (const [t, name, unique, filt, icols] of INDEXES) {
    if (t !== table) continue;
    lines.push(`        builder.HasIndex(${indexExpr(icols)})`);
    if (unique) lines.push("            .IsUnique()");
    if (filt) lines.push(`            .HasFilter("${filt}")`);
    lines.push(`            .HasDatabaseName("${name}");`);
    lines.push("");
  }
  while (lines.at(-1) === "") lines.pop();
  lines.push("    }", "}", "");
  write(path.join(CFG, `${className}Configuration.cs`), lines.join("\n"));
}

const tables = parseColumns();
const classToTable = Object.fromEntries(Object.entries(TABLE_CLASS).map(([t, c]) => [c, t]));
const refs = {};
const colsNav = {};
for (const [dep, fk, prin, depNav, prinNav, one] of FKS) {
  const table = classToTable[dep];
  const col = tables[table].find((c) => c.name === fk);
  (refs[dep] ??= []).push([depNav, prin, !col.nullable]);
  if (one) (refs[prin] ??= []).push([prinNav, dep, false]);
  else (colsNav[prin] ??= []).push([prinNav, dep]);
}
for (const [k, v] of Object.entries(colsNav)) {
  const seen = new Set();
  colsNav[k] = v.filter((item) => {
    const key = item.join("|");
    if (seen.has(key)) return false;
    seen.add(key);
    return true;
  });
}

for (const [table, className] of Object.entries(TABLE_CLASS)) {
  emitEntity(className, table, tables[table], refs[className] ?? [], colsNav[className] ?? []);
  emitConfig(className, table, tables[table]);
}
console.log(`Wrote ${Object.keys(TABLE_CLASS).length} entities and configurations.`);
