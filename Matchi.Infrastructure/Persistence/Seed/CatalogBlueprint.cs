namespace Matchi.Infrastructure.Persistence.Seed;

internal sealed record OptionSeed(string Value, string DisplayName);

internal sealed record AttributeSeed(
    string Code,
    string Name,
    string DataType,
    bool IsRequired,
    OptionSeed[] Options);

internal sealed record ServiceSeed(
    string Slug,
    string Name,
    string? Description,
    AttributeSeed[] Attributes);

internal sealed record ServiceCategorySeed(
    string Slug,
    string Name,
    int DisplayOrder,
    ServiceSeed[] Services);

internal sealed record ProductSeed(string Slug, string Name, string? Description);

internal sealed record ProductCategorySeed(
    string Slug,
    string Name,
    string? ParentSlug,
    int DisplayOrder,
    string? Description,
    ProductSeed[] Products,
    AttributeSeed[] Attributes);

internal static class CatalogBlueprint
{
    private static readonly OptionSeed[] ApplianceBrands =
    [
        O("gree", "گری"),
        O("og-general", "اجنرال"),
        O("lg", "ال‌جی"),
        O("samsung", "سامسونگ"),
        O("hisense", "هایسنس"),
        O("bosch", "بوش"),
        O("other", "سایر")
    ];

    private static readonly OptionSeed[] AcCapacity =
    [
        O("9000", "۹۰۰۰"),
        O("12000", "۱۲۰۰۰"),
        O("18000", "۱۸۰۰۰"),
        O("24000", "۲۴۰۰۰"),
        O("30000", "۳۰۰۰۰"),
        O("other", "سایر")
    ];

    private static readonly OptionSeed[] AreaSize =
    [
        O("under-50", "کمتر از ۵۰ متر"),
        O("50-80", "۵۰ تا ۸۰ متر"),
        O("80-120", "۸۰ تا ۱۲۰ متر"),
        O("120-180", "۱۲۰ تا ۱۸۰ متر"),
        O("over-180", "بیش از ۱۸۰ متر")
    ];

    private static readonly OptionSeed[] RoomCount =
    [
        O("1", "۱"),
        O("2", "۲"),
        O("3", "۳"),
        O("4", "۴"),
        O("5plus", "۵ یا بیشتر")
    ];

    public static readonly ServiceCategorySeed[] ServiceCategories =
    [
        Cat("home-appliance-repair", "تعمیرات لوازم خانگی", 1,
            Svc("repair-refrigerator", "تعمیر یخچال", "عیب‌یابی و تعمیر یخچال خانگی",
                Attr("fridge_brand", "برند", true, ApplianceBrands),
                Attr("fridge_type", "نوع یخچال", false,
                    O("single", "تک‌درب"), O("double", "دوقلو"), O("side-by-side", "ساید بای ساید"), O("mini", "کوچک")),
                Attr("fridge_issue", "مشکل", true,
                    O("not-cooling", "خنک نکردن"), O("noise", "صدای غیرعادی"), O("leak", "آب دادن"),
                    O("not-starting", "روشن نشدن"), O("other", "سایر"))),
            Svc("repair-freezer", "تعمیر فریزر"),
            Svc("repair-washing-machine", "تعمیر ماشین لباسشویی", null,
                Attr("washer_brand", "برند", true, ApplianceBrands),
                Attr("washer_capacity", "ظرفیت", false,
                    O("5kg", "۵ کیلو"), O("7kg", "۷ کیلو"), O("8kg", "۸ کیلو"), O("9kg", "۹ کیلو"), O("other", "سایر")),
                Attr("washer_issue", "مشکل", true,
                    O("not-spinning", "خشک‌کن کار نمی‌کند"), O("leak", "نشتی آب"), O("noise", "صدای غیرعادی"),
                    O("not-starting", "روشن نشدن"), O("error-code", "کد خطا"), O("other", "سایر"))),
            Svc("repair-dishwasher", "تعمیر ماشین ظرفشویی"),
            Svc("repair-gas-stove", "تعمیر اجاق گاز"),
            Svc("repair-microwave", "تعمیر مایکروویو"),
            Svc("repair-vacuum", "تعمیر جاروبرقی"),
            Svc("repair-water-dispenser", "تعمیر آبسردکن"),
            Svc("repair-hood", "تعمیر هود آشپزخانه"),
            Svc("repair-oven", "تعمیر فر توکار"),
            Svc("repair-iron", "تعمیر اتو"),
            Svc("repair-blender", "تعمیر مخلوط‌کن و غذاساز")),
        Cat("plumbing", "تأسیسات و لوله‌کشی", 2,
            Svc("repair-water-leak", "رفع نشتی آب"),
            Svc("repair-faucets", "تعمیر شیرآلات"),
            Svc("install-water-piping", "لوله‌کشی آب"),
            Svc("unclog-drain", "رفع گرفتگی لوله"),
            Svc("repair-water-pump", "تعمیر پمپ آب"),
            Svc("install-toilet", "نصب و تعمیر توالت فرنگی"),
            Svc("install-water-heater-plumbing", "نصب آبگرمکن"),
            Svc("sewer-line-repair", "تعمیر فاضلاب"),
            Svc("install-shower", "نصب دوش و وان"),
            Svc("repair-flush-tank", "تعمیر سیفون و فلاش‌تانک")),
        Cat("electrical", "برق و روشنایی", 3,
            Svc("fix-short-circuit", "رفع اتصالی برق"),
            Svc("install-light-fixture", "نصب چراغ"),
            Svc("install-switch-outlet", "نصب کلید و پریز"),
            Svc("building-wiring", "سیم‌کشی ساختمان"),
            Svc("install-intercom", "نصب آیفون"),
            Svc("repair-electrical-panel", "تعمیر تابلو برق"),
            Svc("install-chandelier", "نصب لوستر"),
            Svc("smart-home-wiring", "سیم‌کشی خانه هوشمند"),
            Svc("earth-leakage-fix", "رفع نشتی جریان"),
            Svc("backup-power-setup", "راه‌اندازی برق اضطراری")),
        Cat("hvac", "سرمایش و گرمایش", 4,
            Svc("repair-air-conditioner", "تعمیر کولر گازی", "عیب‌یابی کولر گازی دیواری و ایستاده",
                Attr("ac_type", "نوع دستگاه", true,
                    O("wall", "دیواری"), O("standing", "ایستاده"), O("portable", "پرتابل")),
                Attr("ac_brand", "برند", false, ApplianceBrands),
                Attr("ac_capacity", "ظرفیت", false, AcCapacity),
                Attr("ac_issue", "مشکل دستگاه", true,
                    O("not-cooling", "خنک نکردن"), O("water-leak", "آب دادن"), O("noise", "صدای غیرعادی"),
                    O("not-starting", "روشن نشدن"), O("icing", "یخ‌زدگی"), O("other", "سایر"))),
            Svc("install-air-conditioner", "نصب کولر گازی"),
            Svc("service-air-conditioner", "سرویس کولر گازی"),
            Svc("repair-evaporative-cooler", "تعمیر کولر آبی"),
            Svc("repair-combi-boiler", "تعمیر پکیج"),
            Svc("service-combi-boiler", "سرویس پکیج"),
            Svc("repair-water-heater", "تعمیر آبگرمکن"),
            Svc("install-radiator", "نصب رادیاتور"),
            Svc("repair-fan-coil", "تعمیر فن‌کویل"),
            Svc("install-split-duct", "نصب داکت اسپلیت"),
            Svc("heater-service", "سرویس بخاری"),
            Svc("floor-heating-repair", "تعمیر گرمایش از کف")),
        Cat("cleaning", "نظافت و خدمات منزل", 5,
            Svc("home-cleaning", "نظافت منزل", null,
                Attr("clean_area", "متراژ تقریبی", true, AreaSize),
                Attr("clean_rooms", "تعداد اتاق", false, RoomCount),
                Attr("clean_type", "نوع نظافت", true,
                    O("regular", "عادی"), O("deep", "عمیق"), O("move-in", "تخلیه و تحویل")),
                Attr("clean_bathrooms", "تعداد سرویس بهداشتی", false,
                    O("1", "۱"), O("2", "۲"), O("3plus", "۳ یا بیشتر"))),
            Svc("office-cleaning", "نظافت محل کار"),
            Svc("stairwell-cleaning", "نظافت راه‌پله"),
            Svc("sofa-cleaning", "شست‌وشوی مبل"),
            Svc("carpet-cleaning", "شست‌وشوی فرش"),
            Svc("window-cleaning", "شست‌وشوی شیشه"),
            Svc("facade-cleaning", "شست‌وشوی نما"),
            Svc("post-construction-cleaning", "نظافت پس از بازسازی")),
        Cat("painting-decoration", "نقاشی و دکوراسیون", 6,
            Svc("building-painting", "نقاشی ساختمان", null,
                Attr("paint_area", "متراژ تقریبی", true, AreaSize),
                Attr("wall_condition", "وضعیت دیوار", false,
                    O("ready", "آماده رنگ"), O("needs-prep", "نیاز به بتونه‌کاری"), O("damaged", "آسیب‌دیده")),
                Attr("paint_type", "نوع رنگ", false,
                    O("acrylic", "اکریلیک"), O("oil", "روغنی"), O("plastic", "پلاستیک"), O("other", "سایر")),
                Attr("paint_rooms", "تعداد اتاق", false, RoomCount)),
            Svc("wallpaper-install", "نصب کاغذ دیواری"),
            Svc("flooring-install", "نصب کف‌پوش"),
            Svc("parquet-install", "نصب پارکت"),
            Svc("interior-remodel", "بازسازی دکوراسیون"),
            Svc("ceiling-design", "طراحی سقف کاذب"),
            Svc("wall-panel-install", "نصب دیوارپوش"),
            Svc("epoxy-floor", "کف‌پوش اپوکسی")),
        Cat("carpentry", "کابینت و نجاری", 7,
            Svc("build-cabinet", "ساخت کابینت"),
            Svc("repair-cabinet", "تعمیر کابینت"),
            Svc("build-wardrobe", "ساخت کمد دیواری"),
            Svc("repair-furniture", "تعمیر مبلمان"),
            Svc("build-table-shelf", "ساخت میز و قفسه"),
            Svc("door-install", "نصب در چوبی"),
            Svc("wood-floor-repair", "تعمیر کف چوبی"),
            Svc("custom-woodwork", "نجاری سفارشی")),
        Cat("glass-window", "شیشه و پنجره", 8,
            Svc("window-install", "نصب پنجره"),
            Svc("glass-replacement", "تعویض شیشه"),
            Svc("double-glazing", "نصب شیشه دوجداره"),
            Svc("mirror-install", "نصب آینه"),
            Svc("balcony-glass", "شیشه بالکن"),
            Svc("window-seal-repair", "تعمیر نوار پنجره"),
            Svc("flyscreen-install", "نصب توری پنجره"),
            Svc("glass-table-top", "شیشه میز و کابینت")),
        Cat("construction", "خدمات ساختمانی", 9,
            Svc("tiling", "کاشی‌کاری"),
            Svc("ceramic-work", "سرامیک‌کاری"),
            Svc("plastering", "گچ‌کاری"),
            Svc("cement-work", "سیمان‌کاری"),
            Svc("masonry", "بنایی"),
            Svc("waterproofing", "عایق‌کاری"),
            Svc("facade-stone", "سنگ نما"),
            Svc("demolition", "تخریب محدود"),
            Svc("concrete-repair", "ترمیم بتن"),
            Svc("scaffolding", "داربست")),
        Cat("moving-transport", "اسباب‌کشی و حمل‌ونقل", 10,
            Svc("home-moving", "اسباب‌کشی منزل"),
            Svc("office-moving", "اسباب‌کشی محل کار"),
            Svc("furniture-transport", "حمل مبلمان"),
            Svc("packing-service", "بسته‌بندی اثاثیه"),
            Svc("piano-moving", "حمل پیانو و وسایل سنگین"),
            Svc("storage-move", "انتقال به انبار"),
            Svc("intercity-moving", "حمل بین‌شهری")),
        Cat("computer-network", "خدمات کامپیوتری و شبکه", 11,
            Svc("repair-computer", "تعمیر کامپیوتر"),
            Svc("install-windows", "نصب ویندوز"),
            Svc("install-software", "نصب نرم‌افزار"),
            Svc("setup-network", "راه‌اندازی شبکه"),
            Svc("fix-network", "رفع مشکل شبکه"),
            Svc("install-printer", "نصب پرینتر"),
            Svc("data-recovery", "بازیابی اطلاعات"),
            Svc("laptop-repair", "تعمیر لپ‌تاپ")),
        Cat("security-systems", "دوربین و سیستم‌های امنیتی", 12,
            Svc("install-cctv", "نصب دوربین مداربسته"),
            Svc("repair-cctv", "تعمیر دوربین مداربسته"),
            Svc("install-alarm", "نصب دزدگیر"),
            Svc("install-access-control", "نصب سیستم کنترل تردد"),
            Svc("install-intercom-video", "نصب آیفون تصویری"),
            Svc("fire-alarm-install", "نصب اعلام حریق")),
        Cat("education", "آموزش", 13,
            Svc("private-tutoring", "تدریس خصوصی"),
            Svc("language-class", "آموزش زبان"),
            Svc("music-lesson", "آموزش موسیقی"),
            Svc("computer-training", "آموزش کامپیوتر"),
            Svc("exam-prep", "آمادگی کنکور"),
            Svc("skill-workshop", "کارگاه مهارت"),
            Svc("driving-lesson", "آموزش رانندگی")),
        Cat("automotive", "خدمات خودرو", 14,
            Svc("car-mechanic", "مکانیکی خودرو"),
            Svc("car-electrical", "برق خودرو"),
            Svc("jump-start", "باتری به باتری"),
            Svc("oil-change", "تعویض روغن"),
            Svc("mobile-tire-repair", "پنچرگیری سیار"),
            Svc("car-inspection", "کارشناسی خودرو"),
            Svc("car-wash", "کارواش در محل"),
            Svc("ac-car-repair", "تعمیر کولر خودرو"),
            Svc("brake-service", "تعمیر ترمز"),
            Svc("battery-replacement", "تعویض باتری")),
        Cat("gardening", "خدمات باغبانی", 15,
            Svc("garden-maintenance", "نگهداری باغچه"),
            Svc("lawn-care", "چمن‌زنی"),
            Svc("tree-pruning", "هرس درخت"),
            Svc("irrigation-install", "نصب آبیاری قطره‌ای"),
            Svc("planting", "کاشت گیاه"),
            Svc("pest-control-garden", "سم‌پاشی فضای سبز"),
            Svc("greenhouse-setup", "راه‌اندازی گلخانه خانگی")),
        Cat("events-catering", "خدمات مراسم و پذیرایی", 16,
            Svc("catering", "پذیرایی و کترینگ"),
            Svc("event-setup", "چیدمان مراسم"),
            Svc("birthday-party", "برگزاری جشن تولد"),
            Svc("photography-event", "عکاسی مراسم"),
            Svc("rental-tables", "اجاره میز و صندلی"),
            Svc("florist-event", "گل‌آرایی مراسم"))
    ];

    public static readonly ProductCategorySeed[] ProductCategories =
    [
        PCat("cooling", "لوازم سرمایشی", null, 1, null,
            [
                P("air-conditioner", "کولر گازی"),
                P("evaporative-cooler", "کولر آبی"),
                P("ac-compressor", "کمپرسور کولر گازی"),
                P("ac-remote", "ریموت کولر گازی"),
                P("ac-filter", "فیلتر کولر گازی"),
                P("ac-copper-pipe", "لوله مسی کولر"),
                P("portable-ac", "کولر پرتابل"),
                P("fan-coil-unit", "فن‌کویل"),
                P("ac-drain-pump", "پمپ درین کولر"),
                P("ac-gas-charge-kit", "کیت شارژ گاز کولر")
            ],
            [
                Attr("cooling_brand", "برند", false, ApplianceBrands),
                Attr("cooling_capacity", "ظرفیت", false, AcCapacity),
                Attr("compressor_type", "نوع کمپرسور", false,
                    O("rotary", "روتاری"), O("scroll", "اسکرول"), O("inverter", "اینورتر")),
                Attr("power_type", "نوع مصرف", false,
                    O("single-phase", "تک‌فاز"), O("three-phase", "سه‌فاز"))
            ]),
        PCat("split-ac", "کولر گازی", "cooling", 1, null,
            [P("wall-split-ac", "اسپلیت دیواری"), P("standing-ac", "کولر ایستاده")],
            [
                Attr("split_brand", "برند", true, ApplianceBrands),
                Attr("split_capacity", "ظرفیت", true, AcCapacity)
            ]),
        PCat("evaporative-coolers", "کولر آبی", "cooling", 2, null,
            [P("cellulosic-cooler", "کولر سلولزی"), P("cooler-pad", "پوشال کولر")], []),
        PCat("ac-parts", "قطعات کولر", "cooling", 3, null,
            [P("ac-capacitor", "خازن کولر"), P("ac-pcb", "برد کولر گازی")], []),
        PCat("heating", "لوازم گرمایشی", null, 2, null,
            [
                P("combi-boiler", "پکیج"),
                P("water-heater", "آبگرمکن"),
                P("radiator", "رادیاتور"),
                P("gas-heater", "بخاری گازی"),
                P("electric-heater", "بخاری برقی"),
                P("floor-heating-pipe", "لوله گرمایش از کف"),
                P("thermostat", "ترموستات"),
                P("boiler-pump", "پمپ پکیج")
            ],
            [
                Attr("heating_fuel", "نوع سوخت", false,
                    O("gas", "گاز"), O("electric", "برق"), O("other", "سایر")),
                Attr("heating_capacity", "ظرفیت", false,
                    O("small", "کوچک"), O("medium", "متوسط"), O("large", "بزرگ"))
            ]),
        PCat("building-electrical", "لوازم برقی ساختمان", null, 3, null,
            [
                P("single-pole-switch", "کلید تک‌پل"),
                P("double-pole-switch", "کلید دوپل"),
                P("power-outlet", "پریز برق"),
                P("surge-protector", "محافظ برق"),
                P("mcb-fuse", "فیوز مینیاتوری"),
                P("power-cable", "کابل برق"),
                P("junction-box", "جعبه تقسیم"),
                P("dimmer-switch", "کلید دیمر"),
                P("usb-outlet", "پریز USB"),
                P("rcd-breaker", "کلید محافظ جان")
            ],
            [
                Attr("amp_rating", "آمپر", false,
                    O("6", "۶ آمپر"), O("10", "۱۰ آمپر"), O("16", "۱۶ آمپر"), O("25", "۲۵ آمپر"), O("32", "۳۲ آمپر")),
                Attr("voltage", "ولتاژ", false, O("220", "۲۲۰ ولت"), O("380", "۳۸۰ ولت"))
            ]),
        PCat("plumbing-supplies", "لوازم تأسیسات", null, 4, null,
            [
                P("water-faucet", "شیر آب"),
                P("hose", "شلنگ"),
                P("water-pump", "پمپ آب"),
                P("float-valve", "فلوتر"),
                P("check-valve", "شیر یک‌طرفه"),
                P("pipe-fitting", "اتصالات لوله"),
                P("pex-pipe", "لوله پنج‌لایه"),
                P("angle-valve", "شیر پیسوار"),
                P("water-filter", "فیلتر آب"),
                P("pressure-reducer", "کاهش‌دهنده فشار")
            ],
            [
                Attr("faucet_type", "نوع شیر", false,
                    O("mixer", "اهرمی"), O("two-handle", "دوپایه"), O("sensor", "چشمی")),
                Attr("material", "جنس", false,
                    O("brass", "برنج"), O("stainless", "استیل"), O("plastic", "پلاستیک")),
                Attr("finish_color", "رنگ", false,
                    O("chrome", "کروم"), O("matte-black", "مشکی مات"), O("gold", "طلایی"))
            ]),
        PCat("kitchen-appliances", "لوازم آشپزخانه", null, 5, null,
            [
                P("range-hood", "هود"),
                P("gas-cooktop", "اجاق گاز رومیزی"),
                P("built-in-oven", "فر توکار"),
                P("dishwasher", "ماشین ظرفشویی"),
                P("refrigerator", "یخچال"),
                P("microwave", "مایکروویو"),
                P("sink", "سینک ظرفشویی"),
                P("garbage-disposal", "خردکن سینک")
            ],
            [Attr("kitchen_brand", "برند", false, ApplianceBrands)]),
        PCat("lighting", "لوازم روشنایی", null, 6, null,
            [
                P("led-bulb", "لامپ ال‌ای‌دی"),
                P("ceiling-light", "چراغ سقفی"),
                P("wall-sconce", "چراغ دیواری"),
                P("chandelier", "لوستر"),
                P("spot-light", "هالوژن و اسپات"),
                P("outdoor-light", "چراغ محوطه"),
                P("emergency-light", "چراغ اضطراری")
            ],
            [
                Attr("wattage", "توان", false,
                    O("5w", "۵ وات"), O("9w", "۹ وات"), O("12w", "۱۲ وات"), O("18w", "۱۸ وات")),
                Attr("color-temp", "دمای رنگ", false,
                    O("warm", "آفتابی"), O("neutral", "طبیعی"), O("cool", "مهتابی"))
            ]),
        PCat("networking-gear", "تجهیزات شبکه", null, 7, null,
            [
                P("router", "روتر"),
                P("network-switch", "سوئیچ شبکه"),
                P("ethernet-cable", "کابل شبکه"),
                P("modem", "مودم"),
                P("access-point", "اکسس پوینت"),
                P("network-card", "کارت شبکه"),
                P("patch-panel", "پچ پنل"),
                P("rj45-connector", "کانکتور RJ45")
            ],
            [
                Attr("net_brand", "برند", false,
                    O("tp-link", "تی‌پی‌لینک"), O("mikrotik", "میکروتیک"), O("cisco", "سیسکو"), O("other", "سایر")),
                Attr("connection_type", "نوع اتصال", false,
                    O("wifi", "وای‌فای"), O("ethernet", "کابل"), O("both", "هردو")),
                Attr("port_count", "تعداد پورت", false,
                    O("4", "۴"), O("8", "۸"), O("16", "۱۶"), O("24", "۲۴")),
                Attr("link_speed", "سرعت شبکه", false,
                    O("100", "۱۰۰ مگ"), O("1000", "گیگابیت"), O("2500", "۲.۵ گیگ"))
            ]),
        PCat("security-gear", "تجهیزات امنیتی", null, 8, null, [], []),
        PCat("cctv", "دوربین مداربسته", "security-gear", 1, null,
            [
                P("cctv-camera", "دوربین مداربسته"),
                P("dome-camera", "دوربین دام"),
                P("bullet-camera", "دوربین بولت"),
                P("ptz-camera", "دوربین چرخشی")
            ],
            [
                Attr("camera_type", "نوع دوربین", true,
                    O("analog", "آنالوگ"), O("ip", "تحت شبکه"), O("wireless", "بی‌سیم")),
                Attr("resolution", "کیفیت تصویر", false,
                    O("2mp", "۲ مگاپیکسل"), O("4mp", "۴ مگاپیکسل"), O("8mp", "۸ مگاپیکسل")),
                Attr("cam_connection", "نوع اتصال", false,
                    O("wired", "سیمی"), O("poe", "PoE"), O("wifi", "وای‌فای")),
                Attr("night_vision", "دید در شب", false,
                    O("ir", "مادون قرمز"), O("color", "رنگی"), O("none", "ندارد"))
            ]),
        PCat("recorders", "دستگاه ضبط", "security-gear", 2, null,
            [
                P("dvr", "دستگاه DVR"),
                P("nvr", "دستگاه NVR"),
                P("surveillance-hdd", "هارد ضبط تصویر")
            ],
            []),
        PCat("alarms", "دزدگیر", "security-gear", 3, null,
            [
                P("motion-sensor", "سنسور حرکتی"),
                P("alarm-system", "دزدگیر اماکن"),
                P("magnetic-contact", "مگنت در"),
                P("siren", "آژیر")
            ],
            []),
        PCat("tools-equipment", "ابزار و تجهیزات", null, 9, null,
            [
                P("drill", "دریل"),
                P("angle-grinder", "فرز"),
                P("toolbox", "جعبه ابزار"),
                P("ladder", "نردبان"),
                P("multimeter", "مولتی‌متر"),
                P("pipe-wrench", "آچار لوله"),
                P("laser-level", "تراز لیزری"),
                P("heat-gun", "سشوار صنعتی")
            ],
            [
                Attr("power_source", "منبع تغذیه", false,
                    O("corded", "برقی"), O("battery", "شارژی"), O("manual", "دستی"))
            ]),
        PCat("building-materials", "مصالح ساختمانی", null, 10, null,
            [
                P("ceramic-tile", "سرامیک"),
                P("wall-tile", "کاشی دیوار"),
                P("cement-bag", "سیمان"),
                P("gypsum", "گچ"),
                P("waterproof-membrane", "عایق رطوبتی"),
                P("grout", "بندکشی"),
                P("primer", "پرایمر"),
                P("adhesive", "چسب کاشی")
            ],
            [
                Attr("pack_size", "بسته‌بندی", false,
                    O("small", "کوچک"), O("medium", "متوسط"), O("large", "بزرگ"))
            ]),
        PCat("bathroom-fixtures", "تجهیزات سرویس بهداشتی", null, 11, null,
            [
                P("toilet", "توالت فرنگی"),
                P("iranian-toilet", "توالت ایرانی"),
                P("washbasin", "روشویی"),
                P("shower-set", "ست دوش"),
                P("bathroom-faucet", "شیرآلات سرویس"),
                P("flush-tank", "فلاش‌تانک"),
                P("bathroom-mirror", "آینه سرویس"),
                P("towel-bar", "جا حوله‌ای")
            ],
            [
                Attr("bath_material", "جنس", false,
                    O("ceramic", "چینی"), O("acrylic", "اکریلیک"), O("steel", "استیل"))
            ]),
        PCat("hardware-fittings", "یراق‌آلات", null, 12, null,
            [
                P("door-handle", "دستگیره در"),
                P("door-lock", "قفل در"),
                P("hinge", "لولا"),
                P("cabinet-handle", "دستگیره کابینت"),
                P("drawer-slide", "ریل کشو"),
                P("window-handle", "دستگیره پنجره"),
                P("door-closer", "آرام‌بند")
            ],
            [
                Attr("finish", "رنگ و روکش", false,
                    O("chrome", "کروم"), O("matte", "مات"), O("bronze", "برنز"))
            ]),
        PCat("auto-parts", "لوازم خودرو", null, 13, null,
            [
                P("car-battery", "باتری خودرو"),
                P("engine-oil", "روغن موتور"),
                P("air-filter-car", "فیلتر هوا"),
                P("wiper-blade", "تیغه برف‌پاک‌کن"),
                P("car-bulb", "لامپ خودرو"),
                P("jump-cables", "کابل باتری به باتری"),
                P("tire-sealant", "اسپری پنچرگیری"),
                P("cabin-filter", "فیلتر کابین"),
                P("spark-plug", "شمع خودرو"),
                P("brake-pad", "لنت ترمز")
            ],
            [
                Attr("vehicle_class", "کلاس خودرو", false,
                    O("sedan", "سواری"), O("suv", "شاسی‌بلند"), O("van", "وانتی"))
            ])
    ];

    private static ServiceCategorySeed Cat(string slug, string name, int order, params ServiceSeed[] services) =>
        new(slug, name, order, services);

    private static ServiceSeed Svc(string slug, string name, string? description = null, params AttributeSeed[] attributes) =>
        new(slug, name, description, attributes);

    private static ProductCategorySeed PCat(
        string slug,
        string name,
        string? parent,
        int order,
        string? description,
        ProductSeed[] products,
        AttributeSeed[] attributes) =>
        new(slug, name, parent, order, description, products, attributes);

    private static ProductSeed P(string slug, string name, string? description = null) =>
        new(slug, name, description);

    private static AttributeSeed Attr(string code, string name, bool required, params OptionSeed[] options) =>
        new(code, name, options.Length > 0 ? "Select" : "Text", required, options);

    private static OptionSeed O(string value, string displayName) => new(value, displayName);
}
