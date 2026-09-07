# Generates Domain entities + EF configurations from MatchiDb baseline metadata.
from __future__ import annotations

from collections import defaultdict
from pathlib import Path

ROOT = Path(r"E:\Armin\Matchi\Matchi-platform")
ENT = ROOT / "Matchi.Domain" / "Entities"
CFG = ROOT / "Matchi.Infrastructure" / "Persistence" / "Configurations"
TSV = ROOT / "tools" / "schema_columns.tsv"

TABLE_CLASS = {
    "BusinessAvailabilities": "BusinessAvailability",
    "Businesses": "Business",
    "BusinessPortfolioMedia": "BusinessPortfolioMedia",
    "BusinessPortfolios": "BusinessPortfolio",
    "BusinessProducts": "BusinessProduct",
    "BusinessProviders": "BusinessProvider",
    "BusinessServiceAreas": "BusinessServiceArea",
    "BusinessServices": "BusinessService",
    "Cancellations": "Cancellation",
    "Complaints": "Complaint",
    "ConversationParticipants": "ConversationParticipant",
    "Conversations": "Conversation",
    "Customers": "Customer",
    "Deals": "Deal",
    "ExecutionAssignments": "ExecutionAssignment",
    "Media": "Media",
    "Messages": "Message",
    "Permissions": "Permission",
    "ProductAttributeOptions": "ProductAttributeOption",
    "ProductAttributes": "ProductAttribute",
    "ProductAttributeValues": "ProductAttributeValue",
    "ProductCategories": "ProductCategory",
    "ProductDeliveries": "ProductDelivery",
    "Products": "Product",
    "ProposalItems": "ProposalItem",
    "Proposals": "Proposal",
    "ProviderAvailabilities": "ProviderAvailability",
    "ProviderCapabilities": "ProviderCapability",
    "ProviderPortfolioMedia": "ProviderPortfolioMedia",
    "ProviderPortfolios": "ProviderPortfolio",
    "ProviderProducts": "ProviderProduct",
    "Providers": "Provider",
    "ProviderServiceAreas": "ProviderServiceArea",
    "ProviderServices": "ProviderService",
    "RequestLocations": "RequestLocation",
    "RequestProductAttributes": "RequestProductAttribute",
    "RequestProducts": "RequestProduct",
    "Requests": "Request",
    "RequestSchedules": "RequestSchedule",
    "RequestServiceAttributes": "RequestServiceAttribute",
    "RequestServices": "RequestService",
    "Reviews": "Review",
    "RolePermissions": "RolePermission",
    "Roles": "Role",
    "ServiceAttributeOptions": "ServiceAttributeOption",
    "ServiceAttributes": "ServiceAttribute",
    "ServiceCategories": "ServiceCategory",
    "ServiceExecutions": "ServiceExecution",
    "Services": "Service",
    "TrustScores": "TrustScore",
    "UserRoles": "UserRole",
    "Users": "User",
    "VerificationDocuments": "VerificationDocument",
    "Verifications": "Verification",
}

# dep_class, fk, prin_class, dep_nav, prin_nav, one_to_one, constraint
FKS = [
    ("BusinessAvailability", "BusinessId", "Business", "Business", "Availabilities", False, "FK_BusinessAvailabilities_Businesses"),
    ("Business", "LogoMediaId", "Media", "LogoMedia", "LogoBusinesses", False, "FK_Businesses_LogoMedia"),
    ("Business", "OwnerUserId", "User", "OwnerUser", "Businesses", False, "FK_Businesses_Users"),
    ("BusinessPortfolioMedia", "MediaId", "Media", "Media", "BusinessPortfolioMedia", False, "FK_BusinessPortfolioMedia_Media"),
    ("BusinessPortfolioMedia", "PortfolioId", "BusinessPortfolio", "Portfolio", "MediaItems", False, "FK_BusinessPortfolioMedia_Portfolios"),
    ("BusinessPortfolio", "BusinessId", "Business", "Business", "Portfolios", False, "FK_BusinessPortfolios_Businesses"),
    ("BusinessProduct", "BusinessId", "Business", "Business", "Products", False, "FK_BusinessProducts_Businesses"),
    ("BusinessProduct", "ProductId", "Product", "Product", "BusinessProducts", False, "FK_BusinessProducts_Products"),
    ("BusinessProvider", "BusinessId", "Business", "Business", "BusinessProviders", False, "FK_BusinessProviders_Businesses"),
    ("BusinessProvider", "ProviderId", "Provider", "Provider", "BusinessProviders", False, "FK_BusinessProviders_Providers"),
    ("BusinessServiceArea", "BusinessId", "Business", "Business", "ServiceAreas", False, "FK_BusinessServiceAreas_Businesses"),
    ("BusinessService", "BusinessId", "Business", "Business", "Services", False, "FK_BusinessServices_Businesses"),
    ("BusinessService", "ServiceId", "Service", "Service", "BusinessServices", False, "FK_BusinessServices_Services"),
    ("Cancellation", "DealId", "Deal", "Deal", "Cancellations", False, "FK_Cancellations_Deals"),
    ("Cancellation", "RequestId", "Request", "Request", "Cancellations", False, "FK_Cancellations_Requests"),
    ("Cancellation", "CancelledByUserId", "User", "CancelledByUser", "Cancellations", False, "FK_Cancellations_Users"),
    ("Complaint", "BusinessId", "Business", "Business", "Complaints", False, "FK_Complaints_Businesses"),
    ("Complaint", "CustomerId", "Customer", "Customer", "Complaints", False, "FK_Complaints_Customers"),
    ("Complaint", "DealId", "Deal", "Deal", "Complaints", False, "FK_Complaints_Deals"),
    ("Complaint", "ProviderId", "Provider", "Provider", "Complaints", False, "FK_Complaints_Providers"),
    ("Complaint", "RequestId", "Request", "Request", "Complaints", False, "FK_Complaints_Requests"),
    ("ConversationParticipant", "ConversationId", "Conversation", "Conversation", "Participants", False, "FK_ConversationParticipants_Conversations"),
    ("ConversationParticipant", "UserId", "User", "User", "ConversationParticipants", False, "FK_ConversationParticipants_Users"),
    ("Conversation", "BusinessId", "Business", "Business", "Conversations", False, "FK_Conversations_Businesses"),
    ("Conversation", "CustomerId", "Customer", "Customer", "Conversations", False, "FK_Conversations_Customers"),
    ("Conversation", "RequestId", "Request", "Request", "Conversations", False, "FK_Conversations_Requests"),
    ("Customer", "UserId", "User", "User", "Customer", True, "FK_Customers_Users"),
    ("Deal", "CustomerId", "Customer", "Customer", "Deals", False, "FK_Deals_Customers"),
    ("Deal", "ProposalId", "Proposal", "Proposal", "Deal", True, "FK_Deals_Proposals"),
    ("Deal", "RequestId", "Request", "Request", "Deals", False, "FK_Deals_Requests"),
    ("ExecutionAssignment", "ProviderId", "Provider", "Provider", "ExecutionAssignments", False, "FK_ExecutionAssignments_Providers"),
    ("ExecutionAssignment", "ServiceExecutionId", "ServiceExecution", "ServiceExecution", "Assignments", False, "FK_ExecutionAssignments_ServiceExecutions"),
    ("Message", "ConversationId", "Conversation", "Conversation", "Messages", False, "FK_Messages_Conversations"),
    ("Message", "SenderUserId", "User", "SenderUser", "SentMessages", False, "FK_Messages_Users"),
    ("ProductAttributeOption", "ProductAttributeId", "ProductAttribute", "ProductAttribute", "Options", False, "FK_ProductAttributeOptions_ProductAttributes"),
    ("ProductAttribute", "ProductCategoryId", "ProductCategory", "ProductCategory", "Attributes", False, "FK_ProductAttributes_ProductCategories"),
    ("ProductAttributeValue", "ProductAttributeId", "ProductAttribute", "ProductAttribute", "Values", False, "FK_ProductAttributeValues_ProductAttributes"),
    ("ProductAttributeValue", "ProductId", "Product", "Product", "AttributeValues", False, "FK_ProductAttributeValues_Products"),
    ("ProductCategory", "ParentId", "ProductCategory", "Parent", "Children", False, "FK_ProductCategories_Parent"),
    ("ProductDelivery", "DealId", "Deal", "Deal", "ProductDeliveries", False, "FK_ProductDeliveries_Deals"),
    ("Product", "CategoryId", "ProductCategory", "Category", "Products", False, "FK_Products_ProductCategories"),
    ("ProposalItem", "ProductId", "Product", "Product", "ProposalItems", False, "FK_ProposalItems_Products"),
    ("ProposalItem", "ProposalId", "Proposal", "Proposal", "Items", False, "FK_ProposalItems_Proposals"),
    ("ProposalItem", "ServiceId", "Service", "Service", "ProposalItems", False, "FK_ProposalItems_Services"),
    ("Proposal", "BusinessId", "Business", "Business", "Proposals", False, "FK_Proposals_Businesses"),
    ("Proposal", "ProviderId", "Provider", "Provider", "Proposals", False, "FK_Proposals_Providers"),
    ("Proposal", "RequestId", "Request", "Request", "Proposals", False, "FK_Proposals_Requests"),
    ("ProviderAvailability", "ProviderId", "Provider", "Provider", "Availabilities", False, "FK_ProviderAvailabilities_Providers"),
    ("ProviderCapability", "ProviderId", "Provider", "Provider", "Capabilities", False, "FK_ProviderCapabilities_Providers"),
    ("ProviderCapability", "ServiceAttributeId", "ServiceAttribute", "ServiceAttribute", "ProviderCapabilities", False, "FK_ProviderCapabilities_ServiceAttributes"),
    ("ProviderPortfolioMedia", "MediaId", "Media", "Media", "ProviderPortfolioMedia", False, "FK_ProviderPortfolioMedia_Media"),
    ("ProviderPortfolioMedia", "PortfolioId", "ProviderPortfolio", "Portfolio", "MediaItems", False, "FK_ProviderPortfolioMedia_Portfolios"),
    ("ProviderPortfolio", "ProviderId", "Provider", "Provider", "Portfolios", False, "FK_ProviderPortfolios_Providers"),
    ("ProviderProduct", "ProductId", "Product", "Product", "ProviderProducts", False, "FK_ProviderProducts_Products"),
    ("ProviderProduct", "ProviderId", "Provider", "Provider", "Products", False, "FK_ProviderProducts_Providers"),
    ("Provider", "UserId", "User", "User", "Provider", True, "FK_Providers_Users"),
    ("ProviderServiceArea", "ProviderId", "Provider", "Provider", "ServiceAreas", False, "FK_ProviderServiceAreas_Providers"),
    ("ProviderService", "ProviderId", "Provider", "Provider", "ProviderServices", False, "FK_ProviderServices_Providers"),
    ("ProviderService", "ServiceId", "Service", "Service", "ProviderServices", False, "FK_ProviderServices_Services"),
    ("RequestLocation", "RequestId", "Request", "Request", "Locations", False, "FK_RequestLocations_Requests"),
    ("RequestProductAttribute", "ProductAttributeId", "ProductAttribute", "ProductAttribute", "RequestProductAttributes", False, "FK_RequestProductAttributes_ProductAttributes"),
    ("RequestProductAttribute", "RequestProductId", "RequestProduct", "RequestProduct", "Attributes", False, "FK_RequestProductAttributes_RequestProducts"),
    ("RequestProduct", "ProductCategoryId", "ProductCategory", "ProductCategory", "RequestProducts", False, "FK_RequestProducts_ProductCategories"),
    ("RequestProduct", "ProductId", "Product", "Product", "RequestProducts", False, "FK_RequestProducts_Products"),
    ("RequestProduct", "RequestId", "Request", "Request", "Products", False, "FK_RequestProducts_Requests"),
    ("Request", "CustomerId", "Customer", "Customer", "Requests", False, "FK_Requests_Customers"),
    ("RequestSchedule", "RequestId", "Request", "Request", "Schedules", False, "FK_RequestSchedules_Requests"),
    ("RequestServiceAttribute", "RequestServiceId", "RequestService", "RequestService", "Attributes", False, "FK_RequestServiceAttributes_RequestServices"),
    ("RequestServiceAttribute", "ServiceAttributeId", "ServiceAttribute", "ServiceAttribute", "RequestServiceAttributes", False, "FK_RequestServiceAttributes_ServiceAttributes"),
    ("RequestService", "RequestId", "Request", "Request", "Services", False, "FK_RequestServices_Requests"),
    ("RequestService", "ServiceId", "Service", "Service", "RequestServices", False, "FK_RequestServices_Services"),
    ("Review", "BusinessId", "Business", "Business", "Reviews", False, "FK_Reviews_Businesses"),
    ("Review", "CustomerId", "Customer", "Customer", "Reviews", False, "FK_Reviews_Customers"),
    ("Review", "DealId", "Deal", "Deal", "Reviews", False, "FK_Reviews_Deals"),
    ("Review", "ProviderId", "Provider", "Provider", "Reviews", False, "FK_Reviews_Providers"),
    ("RolePermission", "PermissionId", "Permission", "Permission", "RolePermissions", False, "FK_RolePermissions_Permissions"),
    ("RolePermission", "RoleId", "Role", "Role", "RolePermissions", False, "FK_RolePermissions_Roles"),
    ("ServiceAttributeOption", "ServiceAttributeId", "ServiceAttribute", "ServiceAttribute", "Options", False, "FK_ServiceAttributeOptions_ServiceAttributes"),
    ("ServiceAttribute", "ServiceId", "Service", "Service", "Attributes", False, "FK_ServiceAttributes_Services"),
    ("ServiceExecution", "BusinessId", "Business", "Business", "ServiceExecutions", False, "FK_ServiceExecutions_Businesses"),
    ("ServiceExecution", "DealId", "Deal", "Deal", "ServiceExecutions", False, "FK_ServiceExecutions_Deals"),
    ("Service", "CategoryId", "ServiceCategory", "Category", "Services", False, "FK_Services_ServiceCategories"),
    ("UserRole", "RoleId", "Role", "Role", "UserRoles", False, "FK_UserRoles_Roles"),
    ("UserRole", "UserId", "User", "User", "UserRoles", False, "FK_UserRoles_Users"),
    ("VerificationDocument", "MediaId", "Media", "Media", "VerificationDocuments", False, "FK_VerificationDocuments_Media"),
    ("VerificationDocument", "VerificationId", "Verification", "Verification", "Documents", False, "FK_VerificationDocuments_Verifications"),
]

INDEXES = [
    ("BusinessAvailabilities", "IX_BusinessAvailabilities", False, None, ["BusinessId", "DayOfWeek"]),
    ("Businesses", "IX_Businesses_OwnerUserId", False, None, ["OwnerUserId"]),
    ("BusinessPortfolioMedia", "IX_BusinessPortfolioMedia_PortfolioId", False, None, ["PortfolioId", "DisplayOrder"]),
    ("BusinessPortfolios", "IX_BusinessPortfolios_BusinessId", False, None, ["BusinessId"]),
    ("BusinessProducts", "UX_BusinessProducts", True, "([IsDeleted]=(0))", ["BusinessId", "ProductId"]),
    ("BusinessProviders", "IX_BusinessProviders_BusinessId", False, None, ["BusinessId"]),
    ("BusinessProviders", "IX_BusinessProviders_ProviderId", False, None, ["ProviderId"]),
    ("BusinessProviders", "UX_BusinessProviders_Active", True, "([IsDeleted]=(0))", ["BusinessId", "ProviderId"]),
    ("BusinessServiceAreas", "IX_BusinessServiceAreas", False, None, ["BusinessId", "City", "District"]),
    ("BusinessServices", "UX_BusinessServices", True, "([IsDeleted]=(0))", ["BusinessId", "ServiceId"]),
    ("Cancellations", "IX_Cancellations_DealId", False, None, ["DealId"]),
    ("Cancellations", "IX_Cancellations_RequestId", False, None, ["RequestId"]),
    ("Complaints", "IX_Complaints_DealId_Status", False, None, ["DealId", "Status"]),
    ("Complaints", "IX_Complaints_RequestId_Status", False, None, ["RequestId", "Status"]),
    ("ConversationParticipants", "IX_ConversationParticipants_ConversationId", False, None, ["ConversationId"]),
    ("Conversations", "IX_Conversations_RequestId", False, None, ["RequestId"]),
    ("Customers", "UX_Customers_UserId", True, "([IsDeleted]=(0))", ["UserId"]),
    ("Deals", "IX_Deals_CustomerId_Status", False, None, ["CustomerId", "Status"]),
    ("Deals", "IX_Deals_RequestId_Status", False, None, ["RequestId", "Status"]),
    ("Deals", "UX_Deals_ProposalId", True, None, ["ProposalId"]),
    ("ExecutionAssignments", "IX_ExecutionAssignments_ProviderId", False, None, ["ProviderId", "Status"]),
    ("Media", "UX_Media_StorageKey", True, None, ["StorageKey"]),
    ("Messages", "IX_Messages_ConversationId_CreatedAt", False, None, ["ConversationId", "CreatedAt"]),
    ("Permissions", "UX_Permissions_Code", True, None, ["Code"]),
    ("Permissions", "UX_Permissions_Name", True, None, ["Name"]),
    ("ProductAttributeOptions", "UX_ProductAttributeOptions_Value", True, "([IsDeleted]=(0))", ["ProductAttributeId", "Value"]),
    ("ProductAttributes", "UX_ProductAttributes_Code", True, "([IsDeleted]=(0))", ["ProductCategoryId", "Code"]),
    ("ProductAttributeValues", "UX_ProductAttributeValues_ProductAttribute", True, "([IsDeleted]=(0))", ["ProductId", "ProductAttributeId"]),
    ("ProductCategories", "IX_ProductCategories_ParentId", False, None, ["ParentId"]),
    ("ProductCategories", "UX_ProductCategories_Slug", True, "([IsDeleted]=(0))", ["Slug"]),
    ("ProductDeliveries", "IX_ProductDeliveries_DealId", False, None, ["DealId"]),
    ("Products", "UX_Products_SKU", True, "([SKU] IS NOT NULL AND [IsDeleted]=(0))", ["SKU"]),
    ("Products", "UX_Products_Slug", True, "([Slug] IS NOT NULL AND [IsDeleted]=(0))", ["Slug"]),
    ("ProposalItems", "IX_ProposalItems_ProductId", False, None, ["ProductId"]),
    ("ProposalItems", "IX_ProposalItems_ProposalId", False, None, ["ProposalId", "DisplayOrder"]),
    ("ProposalItems", "IX_ProposalItems_ServiceId", False, None, ["ServiceId"]),
    ("Proposals", "IX_Proposals_BusinessId", False, None, ["BusinessId"]),
    ("Proposals", "IX_Proposals_ProviderId", False, None, ["ProviderId"]),
    ("Proposals", "IX_Proposals_RequestId_Status", False, None, ["RequestId", "Status", "CreateDate"]),
    ("ProviderAvailabilities", "IX_ProviderAvailabilities", False, None, ["ProviderId", "DayOfWeek"]),
    ("ProviderCapabilities", "IX_ProviderCapabilities", False, None, ["ProviderId", "ServiceAttributeId"]),
    ("ProviderPortfolioMedia", "IX_ProviderPortfolioMedia_PortfolioId", False, None, ["PortfolioId", "DisplayOrder"]),
    ("ProviderPortfolios", "IX_ProviderPortfolios_ProviderId", False, None, ["ProviderId"]),
    ("ProviderProducts", "UX_ProviderProducts", True, "([IsDeleted]=(0))", ["ProviderId", "ProductId"]),
    ("Providers", "UX_Providers_UserId", True, "([IsDeleted]=(0))", ["UserId"]),
    ("ProviderServiceAreas", "IX_ProviderServiceAreas", False, None, ["ProviderId", "City", "District"]),
    ("ProviderServices", "UX_ProviderServices", True, "([IsDeleted]=(0))", ["ProviderId", "ServiceId"]),
    ("RequestLocations", "IX_RequestLocations_RequestId", False, None, ["RequestId"]),
    ("RequestProductAttributes", "UX_RequestProductAttributes", True, "([IsDeleted]=(0))", ["RequestProductId", "ProductAttributeId"]),
    ("RequestProducts", "IX_RequestProducts_CategoryId", False, None, ["ProductCategoryId"]),
    ("RequestProducts", "IX_RequestProducts_ProductId", False, None, ["ProductId"]),
    ("RequestProducts", "IX_RequestProducts_RequestId", False, None, ["RequestId"]),
    ("Requests", "IX_Requests_CustomerId_Status", False, None, ["CustomerId", "Status", "CreateDate"]),
    ("RequestSchedules", "IX_RequestSchedules_RequestId_Date", False, None, ["RequestId", "Date"]),
    ("RequestServiceAttributes", "UX_RequestServiceAttributes", True, "([IsDeleted]=(0))", ["RequestServiceId", "ServiceAttributeId"]),
    ("RequestServices", "IX_RequestServices_RequestId", False, None, ["RequestId"]),
    ("RequestServices", "IX_RequestServices_ServiceId", False, None, ["ServiceId"]),
    ("Reviews", "IX_Reviews_DealId", False, None, ["DealId"]),
    ("Roles", "UX_Roles_Code", True, None, ["Code"]),
    ("Roles", "UX_Roles_Name", True, None, ["Name"]),
    ("ServiceAttributeOptions", "UX_ServiceAttributeOptions_Value", True, "([IsDeleted]=(0))", ["ServiceAttributeId", "Value"]),
    ("ServiceAttributes", "UX_ServiceAttributes_Code", True, "([IsDeleted]=(0))", ["ServiceId", "Code"]),
    ("ServiceCategories", "UX_ServiceCategories_Slug", True, "([IsDeleted]=(0))", ["Slug"]),
    ("ServiceExecutions", "IX_ServiceExecutions_DealId", False, None, ["DealId"]),
    ("Services", "UX_Services_Slug", True, "([Slug] IS NOT NULL AND [IsDeleted]=(0))", ["Slug"]),
    ("TrustScores", "IX_TrustScores_Entity", False, None, ["EntityType", "EntityId", "CalculatedAt"]),
    ("Users", "UX_Users_Mobile", True, "([IsDeleted]=(0))", ["Mobile"]),
    ("VerificationDocuments", "IX_VerificationDocuments_VerificationId", False, None, ["VerificationId"]),
    ("Verifications", "IX_Verifications_Entity", False, None, ["EntityType", "EntityId", "Status"]),
]

CHECKS = [
    ("BusinessAvailabilities", "CK_BusinessAvailabilities_DayOfWeek", "[DayOfWeek]>=(0) AND [DayOfWeek]<=(6)"),
    ("BusinessAvailabilities", "CK_BusinessAvailabilities_Time", "[TimeFrom]<[TimeTo]"),
    ("Businesses", "CK_Businesses_Rating", "[Rating]>=(0) AND [Rating]<=(5)"),
    ("BusinessProducts", "CK_BusinessProducts_LeadTimeDays", "[LeadTimeDays] IS NULL OR [LeadTimeDays]>=(0)"),
    ("BusinessProducts", "CK_BusinessProducts_MinOrderQuantity", "[MinOrderQuantity] IS NULL OR [MinOrderQuantity]>(0)"),
    ("BusinessProducts", "CK_BusinessProducts_Price", "[Price] IS NULL OR [Price]>=(0)"),
    ("BusinessServices", "CK_BusinessServices_PriceRange", "[MinPrice] IS NULL OR [MaxPrice] IS NULL OR [MinPrice]<=[MaxPrice]"),
    ("Deals", "CK_Deals_TotalPrice", "[TotalPrice]>=(0)"),
    ("Media", "CK_Media_Size", "[Size]>=(0)"),
    ("ProposalItems", "CK_ProposalItems_Prices", "[UnitPrice]>=(0) AND [TotalPrice]>=(0)"),
    ("ProposalItems", "CK_ProposalItems_Quantity", "[Quantity]>(0)"),
    ("ProposalItems", "CK_ProposalItems_Type", "[ItemType]=N'Product' AND [ProductId] IS NOT NULL AND [ServiceId] IS NULL OR [ItemType]=N'Service' AND [ProductId] IS NULL AND [ServiceId] IS NOT NULL"),
    ("Proposals", "CK_Proposals_Party", "[BusinessId] IS NOT NULL AND [ProviderId] IS NULL OR [BusinessId] IS NULL AND [ProviderId] IS NOT NULL"),
    ("Proposals", "CK_Proposals_Prices", "[TotalPrice]>=(0) AND [DeliveryFee]>=(0)"),
    ("Proposals", "CK_Proposals_Time", "[ProposedTimeFrom] IS NULL OR [ProposedTimeTo] IS NULL OR [ProposedTimeFrom]<[ProposedTimeTo]"),
    ("ProviderAvailabilities", "CK_ProviderAvailabilities_DayOfWeek", "[DayOfWeek]>=(0) AND [DayOfWeek]<=(6)"),
    ("ProviderAvailabilities", "CK_ProviderAvailabilities_Time", "[TimeFrom]<[TimeTo]"),
    ("ProviderProducts", "CK_ProviderProducts_LeadTimeDays", "[LeadTimeDays] IS NULL OR [LeadTimeDays]>=(0)"),
    ("ProviderProducts", "CK_ProviderProducts_MinOrderQuantity", "[MinOrderQuantity] IS NULL OR [MinOrderQuantity]>(0)"),
    ("ProviderProducts", "CK_ProviderProducts_Price", "[Price] IS NULL OR [Price]>=(0)"),
    ("Providers", "CK_Providers_Rating", "[Rating]>=(0) AND [Rating]<=(5)"),
    ("RequestProducts", "CK_RequestProducts_Quantity", "[Quantity]>(0)"),
    ("Requests", "CK_Requests_RequestType", "[RequestType]=N'Hybrid' OR [RequestType]=N'Service' OR [RequestType]=N'Product'"),
    ("RequestSchedules", "CK_RequestSchedules_Time", "[TimeFrom] IS NULL OR [TimeTo] IS NULL OR [TimeFrom]<[TimeTo]"),
    ("RequestServices", "CK_RequestServices_Quantity", "[Quantity]>(0)"),
    ("Reviews", "CK_Reviews_Rating", "[Rating]>=(1) AND [Rating]<=(5)"),
    ("Reviews", "CK_Reviews_Target", "[BusinessId] IS NOT NULL OR [ProviderId] IS NOT NULL"),
    ("ServiceExecutions", "CK_ServiceExecutions_Time", "[ScheduledTimeFrom] IS NULL OR [ScheduledTimeTo] IS NULL OR [ScheduledTimeFrom]<[ScheduledTimeTo]"),
    ("TrustScores", "CK_TrustScores_Score", "[Score]>=(0) AND [Score]<=(100)"),
]

PKS = {t: ["Id"] for t in TABLE_CLASS}
PKS["UserRoles"] = ["UserId", "RoleId"]
PKS["RolePermissions"] = ["RoleId", "PermissionId"]

PUBLIC_SETTER = {"Role", "Permission", "UserRole", "RolePermission"}

# Extra domain methods / constructors for key entities
SPECIAL_ENTITIES = {
    "User", "Provider", "Business", "Customer", "Service", "ServiceCategory",
    "ProviderService", "BusinessProvider", "Request", "RequestLocation",
    "RequestServiceAttribute",
}


def parse_columns():
    tables = defaultdict(list)
    for line in TSV.read_text(encoding="utf-8").splitlines():
        if not line.strip():
            continue
        parts = line.split("\t")
        table, col, ord_, dtype, maxlen, prec, scale, nullable, default = (parts + [""] * 9)[:9]
        tables[table].append({
            "name": col,
            "ord": int(ord_),
            "dtype": dtype,
            "maxlen": maxlen,
            "prec": prec,
            "scale": scale,
            "nullable": nullable == "YES",
            "default": default,
        })
    return tables


def csharp_type(col):
    n = col["nullable"]
    dt = col["dtype"]
    mapping = {
        "bigint": "long",
        "int": "int",
        "tinyint": "byte",
        "bit": "bool",
        "decimal": "decimal",
        "datetime2": "DateTime",
        "date": "DateOnly",
        "time": "TimeSpan",
        "nvarchar": "string",
    }
    t = mapping[dt]
    if t == "string":
        return "string?" if n else "string"
    return f"{t}?" if n else t


def default_csharp(col):
    d = col["default"]
    if not d:
        return None
    d = d.strip()
    if col["dtype"] == "bit":
        if d in ("((0))", "(0)"):
            return "false"
        if d in ("((1))", "(1)"):
            return "true"
    if d in ("((0))", "(0)"):
        return "0" if col["dtype"] != "decimal" else "0m"
    if d in ("((1))", "(1)"):
        return "1m" if col["dtype"] == "decimal" else "1"
    if d == "(N'Active')":
        return '"Active"'
    if d == "(N'Open')":
        return '"Open"'
    if d == "(N'Pending')":
        return '"Pending"'
    if d == "(N'Assigned')":
        return '"Assigned"'
    return None


def base_class(cols, class_name):
    names = {c["name"] for c in cols}
    if class_name in ("UserRole", "RolePermission"):
        return None
    if "IsDeleted" in names:
        return "AuditableEntity"
    if "CreateDate" in names and "UpdateDate" in names:
        return "TimestampedEntity"
    return "Entity"


def inherited_props(base):
    if base == "AuditableEntity":
        return {"Id", "CreateDate", "UpdateDate", "IsDeleted"}
    if base == "TimestampedEntity":
        return {"Id", "CreateDate", "UpdateDate"}
    if base == "Entity":
        return {"Id"}
    return set()


def write(path: Path, content: str):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.replace("\r\n", "\n"), encoding="utf-8")


def emit_entity(class_name, table, cols, navs_ref, navs_col):
    base = base_class(cols, class_name)
    skip = inherited_props(base)
    pub = class_name in PUBLIC_SETTER
    setter = "set" if pub else "private set"
    lines = ["using Matchi.Domain.Common;", "", "namespace Matchi.Domain.Entities;", ""]
    if base:
        lines.append(f"public class {class_name} : {base}")
    else:
        lines.append(f"public class {class_name}")
    lines.append("{")

    props = [c for c in cols if c["name"] not in skip]
    for c in props:
        t = csharp_type(c)
        init = ""
        if t == "string":
            init = " = null!"
        d = default_csharp(c)
        if d is not None and t not in ("string", "string?"):
            # bool/numeric defaults as field initializers
            if t == "bool":
                init = f" = {d}"
            elif t in ("int", "long", "byte", "decimal") and c["name"] not in ("Id",):
                init = f" = {d}"
        if t == "string" and d and d.startswith('"'):
            init = f" = {d}"
        if init:
            lines.append(f"    public {t} {c['name']} {{ get; {setter}; }}{init};")
        else:
            lines.append(f"    public {t} {c['name']} {{ get; {setter}; }}")
        lines.append("")

    # navigations: refs first then collections
    for nav, typ, required in navs_ref:
        if required:
            lines.append(f"    public {typ} {nav} {{ get; {setter}; }} = null!;")
        else:
            lines.append(f"    public {typ}? {nav} {{ get; {setter}; }}")
        lines.append("")
    for nav, typ in navs_col:
        lines.append(f"    public ICollection<{typ}> {nav} {{ get; {setter}; }} = new List<{typ}>();")
        lines.append("")

    if not pub:
        lines.append(f"    private {class_name}()")
        lines.append("    {")
        lines.append("    }")
        lines.append("")
        ctor = build_ctor(class_name, props)
        if ctor:
            lines.extend(ctor)
            lines.append("")
        extra = extra_methods(class_name)
        if extra:
            lines.extend(extra)

    # trim last blank
    while lines and lines[-1] == "":
        lines.pop()
    lines.append("}")
    lines.append("")
    write(ENT / f"{class_name}.cs", "\n".join(lines))


def build_ctor(class_name, props):
    # required columns without DB default (excluding identity-like)
    req = []
    for c in props:
        if c["nullable"]:
            continue
        if default_csharp(c) is not None:
            continue
        if c["dtype"] in ("datetime2", "date", "time") and c["default"]:
            continue
        if c["name"] in ("CreateDate", "UpdateDate", "IsDeleted", "Id"):
            continue
        req.append(c)
    if not req and class_name not in ("Provider", "Business", "Customer", "User", "Service", "ServiceCategory", "ProviderService", "BusinessProvider"):
        return None

    # special required constructors
    if class_name == "User":
        return [
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
        ]
    if class_name == "Provider":
        return [
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
            "        Status = \"Active\";",
            "    }",
        ]
    if class_name == "Business":
        return [
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
            "        Status = \"Active\";",
            "    }",
        ]
    if class_name == "Customer":
        return [
            "    public Customer(long userId)",
            "    {",
            "        UserId = userId;",
            "    }",
        ]
    if class_name == "Service":
        return [
            "    public Service(string name, long categoryId, string? slug = null, string? description = null, int displayOrder = 0)",
            "    {",
            "        Name = name;",
            "        CategoryId = categoryId;",
            "        Slug = slug;",
            "        Description = description;",
            "        DisplayOrder = displayOrder;",
            "        IsActive = true;",
            "    }",
        ]
    if class_name == "ServiceCategory":
        return [
            "    public ServiceCategory(string name, string slug, int displayOrder = 0)",
            "    {",
            "        Name = name;",
            "        Slug = slug;",
            "        DisplayOrder = displayOrder;",
            "        IsActive = true;",
            "    }",
        ]
    if class_name == "ProviderService":
        return [
            "    public ProviderService(long providerId, long serviceId)",
            "    {",
            "        ProviderId = providerId;",
            "        ServiceId = serviceId;",
            "        IsActive = true;",
            "    }",
        ]
    if class_name == "BusinessProvider":
        return [
            "    public BusinessProvider(long businessId, long providerId, string role = \"Member\")",
            "    {",
            "        BusinessId = businessId;",
            "        ProviderId = providerId;",
            "        Role = role;",
            "        Status = \"Active\";",
            "        JoinedAt = DateTime.UtcNow;",
            "    }",
        ]
    if class_name == "Request":
        return [
            "    public Request(long customerId, string requestType, string title, string? description = null)",
            "    {",
            "        CustomerId = customerId;",
            "        RequestType = requestType;",
            "        Title = title;",
            "        Description = description;",
            "        Status = \"Open\";",
            "    }",
        ]
    if class_name == "RequestLocation":
        return [
            "    public RequestLocation(long requestId, decimal? lat = null, decimal? lng = null, string? address = null)",
            "    {",
            "        RequestId = requestId;",
            "        Lat = lat;",
            "        Lng = lng;",
            "        Address = address;",
            "    }",
        ]
    if class_name == "RequestServiceAttribute":
        return [
            "    public RequestServiceAttribute(long requestServiceId, long serviceAttributeId, string? value = null)",
            "    {",
            "        RequestServiceId = requestServiceId;",
            "        ServiceAttributeId = serviceAttributeId;",
            "        Value = value;",
            "    }",
        ]

    params = []
    assigns = []
    for c in req:
        t = csharp_type(c)
        pname = c["name"][0].lower() + c["name"][1:]
        params.append(f"{t} {pname}")
        assigns.append(f"        {c['name']} = {pname};")
    if not params:
        return None
    return [
        f"    public {class_name}({', '.join(params)})",
        "    {",
        *assigns,
        "    }",
    ]


def extra_methods(class_name):
    return []


def index_expr(cols):
    if len(cols) == 1:
        return f"x => x.{cols[0]}"
    inner = ", ".join(f"x.{c}" for c in cols)
    return f"x => new {{ {inner} }}"


def emit_config(class_name, table, cols):
    base = base_class(cols, class_name)
    checks = [(n, sql) for t, n, sql in CHECKS if t == table]
    lines = [
        "using Matchi.Domain.Entities;",
        "using Microsoft.EntityFrameworkCore;",
        "using Microsoft.EntityFrameworkCore.Metadata.Builders;",
        "",
        "namespace Matchi.Infrastructure.Persistence.Configurations;",
        "",
        f"public class {class_name}Configuration : IEntityTypeConfiguration<{class_name}>",
        "{",
        f"    public void Configure(EntityTypeBuilder<{class_name}> builder)",
        "    {",
    ]
    if checks:
        lines.append(f'        builder.ToTable("{table}", t =>')
        lines.append("        {")
        for n, sql in checks:
            sql_esc = sql.replace('"', '\\"')
            lines.append(f'            t.HasCheckConstraint("{n}", "{sql_esc}");')
        lines.append("        });")
    else:
        lines.append(f'        builder.ToTable("{table}");')
    lines.append("")

    pk = PKS[table]
    pk_name = f"PK_{table}"
    if pk == ["Id"]:
        lines.append(f'        builder.HasKey(x => x.Id).HasName("{pk_name}");')
        lines.append("        builder.Property(x => x.Id).ValueGeneratedOnAdd();")
    else:
        inner = ", ".join(f"x.{c}" for c in pk)
        lines.append(f'        builder.HasKey(x => new {{ {inner} }}).HasName("{pk_name}");')
    lines.append("")

    skip_cfg = inherited_props(base)
    if base == "AuditableEntity":
        lines.append("        builder.ConfigureAuditable();")
        lines.append("")
    elif base == "TimestampedEntity":
        lines.append("        builder.ConfigureTimestamped();")
        lines.append("")
    else:
        names = {c["name"] for c in cols}
        if "CreateDate" in names:
            lines.append("        builder.Property(x => x.CreateDate)")
            lines.append("            .IsRequired()")
            lines.append('            .HasDefaultValueSql("sysutcdatetime()");')
            lines.append("")
            skip_cfg = skip_cfg | {"CreateDate"}
    # still configure CreateDate on UserRole
    for c in cols:
        if c["name"] in skip_cfg:
            continue
        if c["name"] == "Id":
            continue
        p = f"        builder.Property(x => x.{c['name']})"
        chain = []
        if not c["nullable"]:
            chain.append("IsRequired()")
        if c["dtype"] == "nvarchar":
            if c["maxlen"] == "-1":
                chain.append('HasColumnType("nvarchar(max)")')
            elif c["maxlen"]:
                chain.append(f"HasMaxLength({c['maxlen']})")
        if c["dtype"] == "decimal":
            chain.append(f"HasPrecision({c['prec']}, {c['scale']})")
        if c["dtype"] == "tinyint":
            chain.append('HasColumnType("tinyint")')
        if c["dtype"] == "date":
            chain.append('HasColumnType("date")')
        if c["dtype"] == "time":
            chain.append('HasColumnType("time")')
        d = c["default"]
        if d:
            dc = default_csharp(c)
            if dc is not None:
                if dc in ("true", "false") or dc.endswith("m") or dc.replace("-", "").isdigit() or dc.startswith('"'):
                    chain.append(f"HasDefaultValue({dc})")
            elif "sysutcdatetime" in d.lower():
                chain.append('HasDefaultValueSql("sysutcdatetime()")')
        if chain:
            lines.append(p)
            for i, ch in enumerate(chain):
                semi = ";" if i == len(chain) - 1 else ""
                lines.append(f"            .{ch}{semi}")
        lines.append("")

    # relationships
    for dep, fk, prin, dep_nav, prin_nav, one, cname in FKS:
        if dep != class_name:
            continue
        lines.append(f"        builder.HasOne(x => x.{dep_nav})")
        if one:
            lines.append(f"            .WithOne(x => x.{prin_nav})")
            lines.append(f"            .HasForeignKey<{class_name}>(x => x.{fk})")
        else:
            lines.append(f"            .WithMany(x => x.{prin_nav})")
            lines.append(f"            .HasForeignKey(x => x.{fk})")
        lines.append(f'            .HasConstraintName("{cname}")')
        lines.append("            .OnDelete(DeleteBehavior.NoAction);")
        lines.append("")

    for t, name, unique, filt, icols in INDEXES:
        if t != table:
            continue
        lines.append(f"        builder.HasIndex({index_expr(icols)})")
        if unique:
            lines.append("            .IsUnique()")
        if filt:
            lines.append(f'            .HasFilter("{filt}")')
        lines.append(f'            .HasDatabaseName("{name}");')
        lines.append("")

    while lines and lines[-1] == "":
        lines.pop()
    lines.append("    }")
    lines.append("}")
    lines.append("")
    write(CFG / f"{class_name}Configuration.cs", "\n".join(lines))


def main():
    tables = parse_columns()
    class_to_table = {v: k for k, v in TABLE_CLASS.items()}

    refs = defaultdict(list)
    cols_nav = defaultdict(list)
    for dep, fk, prin, dep_nav, prin_nav, one, _ in FKS:
        # is required? if FK column nullable
        table = class_to_table[dep]
        col = next(c for c in tables[table] if c["name"] == fk)
        refs[dep].append((dep_nav, prin, not col["nullable"]))
        if one:
            refs[prin].append((prin_nav, dep, False))
        else:
            cols_nav[prin].append((prin_nav, dep))

    # dedupe collections
    for k, v in list(cols_nav.items()):
        seen = set()
        uniq = []
        for item in v:
            if item in seen:
                continue
            seen.add(item)
            uniq.append(item)
        cols_nav[k] = uniq

    for table, class_name in TABLE_CLASS.items():
        emit_entity(class_name, table, tables[table], refs.get(class_name, []), cols_nav.get(class_name, []))
        emit_config(class_name, table, tables[table])

    print(f"Wrote {len(TABLE_CLASS)} entities and configurations.")


if __name__ == "__main__":
    main()
