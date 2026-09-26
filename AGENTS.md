<div dir="rtl" lang="fa" align="right">

# راهنمای مشارکت در پروژه Sevart

## محدوده و ساختار مخزن

- ریشهٔ مخزن: `D:\projects\shop`
- Frontend در `shop-client/` قرار دارد.
- Backend در `shop-server/` قرار دارد.
- مستندات در `docs/` قرار دارند.
- پروژه یک فروشگاه فارسی و RTL برای لوازم قنادی است.

ساختار اصلی:

| مسیر | مسئولیت |
| --- | --- |
| `shop-client/` | Angular Storefront، Admin و SSR |
| `shop-server/Sevart.Domain/` | Entity، Enum و Business Rule |
| `shop-server/Sevart.Application/` | قراردادها و Use Caseهای Application |
| `shop-server/Sevart.Infrastructure/` | EF Core، Repository، Configuration و Migration |
| `shop-server/Sevart.Api/` | Controller، HTTP Contract و پیکربندی API |
| `docs/` | Context، Architecture، UI و Roadmap |

## فناوری‌ها

### Frontend

- Angular `22.0.x`
- Angular Material و CDK `22.0.4`
- Angular SSR `22.0.5` و Express `5.1.0`
- TypeScript `~6.0.2`
- RxJS `~7.8.0`
- SCSS و Tailwind CSS `4.3.2`
- Vazirmatn `33.0.3`
- npm `11.16.0`
- Vitest `4.0.8`

### Backend

- .NET `10.0`
- ASP.NET Core Web API
- Entity Framework Core `10.0.x`
- SQL Server
- OpenAPI و Swagger
- Clean Architecture

## دستورات Frontend

همهٔ دستورهای npm را از `shop-client/` اجرا کنید:

```bash
npm ci
npm start
npm run build
npm run watch
npm test
npm run serve:ssr:shop-client
```

- پس از تغییر کد Frontend، `npm run build` اجرا شود.
- برای تغییرات دارای تست مرتبط، `npm test` نیز اجرا شود.
- Build فعلی بعضی صفحات Prerender ممکن است به API Development وابسته باشد؛ شکست اتصال API را از خطای TypeScript تفکیک کنید.

## دستورات Backend

دستورها را از `shop-server/` اجرا کنید:

```bash
dotnet restore Sevart.slnx
dotnet build Sevart.slnx
dotnet run --project Sevart.Api/Sevart.Api.csproj
```

- پس از تغییر کد Backend، `dotnet build Sevart.slnx` اجرا شود.
- پیش از ساخت Migration، تغییر Schema، FK، Index و Unique Constraint بررسی و تأیید شود.
- Connection String و Secret واقعی وارد Source Control نشوند.

## قواعد عمومی تغییرات

- فقط فایل‌های مرتبط با درخواست را تغییر دهید.
- از Refactor یا Format گستردهٔ فایل‌های نامرتبط خودداری کنید.
- ساختار پوشه‌ها و نام Routeها بدون نیاز روشن تغییر نکنند.
- تغییرات مرحله‌ای، قابل تست و کوچک باقی بمانند.
- نام‌ها واضح، Domain-oriented و قابل فهم برای Developer دیگر باشند.
- از Abstraction بدون نیاز واقعی خودداری کنید.
- کامنت فارسی جدید داخل کد اضافه نکنید.
- تصمیم مهم معماری یا قرارداد جدید در سند مرتبط ثبت شود.

## قواعد Frontend

- از Standalone Components استفاده کنید.
- الگوهای Angular 22 شامل `inject()`، `signal()`، `computed()` و `toSignal()` را در محل مناسب ادامه دهید.
- از `@if`، `@for` و `@switch` استفاده کنید.
- در کد جدید از `*ngFor` استفاده نکنید.
- Lazy Loading موجود با `loadComponent` و `loadChildren` حفظ شود.
- RTL، فارسی، Responsive Design و Dark Theme الزامی‌اند.
- برای CSS از Propertyهای منطقی مانند `margin-inline` و `padding-inline` استفاده کنید.
- Component جدید باید حالت‌های Loading، Empty، Error و Success موردنیاز را پوشش دهد.
- دسترسی به `window`، `document`، `localStorage` و `sessionStorage` باید SSR-safe باشد.
- برای APIهای مرورگر از `PLATFORM_ID` و `isPlatformBrowser` استفاده کنید.
- برای DOM، `DOCUMENT` را Inject کنید.
- URL API از Environment خوانده شود.

## قواعد Backend و Clean Architecture

جهت وابستگی‌ها:

- `Application -> Domain`
- `Infrastructure -> Application + Domain`
- `Api -> Application + Infrastructure`
- `Domain` نباید به Infrastructure یا API وابسته باشد.

مسئولیت لایه‌ها:

- Domain: Business Rule، Invariant و رفتار Entity
- Application: Use Case و orchestration
- Infrastructure: EF Core، Repository و سرویس‌های خارجی
- API: HTTP، Authentication/Authorization، Request/Response Mapping

Controllerها نباید محل اصلی Business Logic باشند. در وضعیت فعلی بخشی از orchestration داخل Controllerهاست؛ هنگام Feature مرتبط، این مسئولیت به‌تدریج به Application منتقل شود و بازنویسی یک‌جای نامرتبط انجام نشود.

## قواعد Domain

- رفتارهایی مانند Publish، Archive و Update از طریق متد Domain انجام شوند.
- Setter عمومی برای تغییر مستقیم State اضافه نشود مگر دلیل مشخصی وجود داشته باشد.
- Constructor و متدهای Domain از Invariantها محافظت کنند.
- Collection داخلی mutable و خروجی عمومی Read-only باقی بماند.
- Product Optionها generic باشند و به Color یا Size محدود نشوند.
- `ProductOptionInputType` شامل Select، Radio و Color است.
- حذف Option یا Value موجود با رفتار فعال/غیرفعال‌سازی فعلی هماهنگ باشد.
- Stock در Storefront نمایش داده نشود.

## قواعد EF Core و Database

- Configuration هر Entity در `Infrastructure/Persistence/Configurations` نگهداری شود.
- Relationship و Delete Behavior صریح باشد.
- تغییر Column، FK، Index یا Constraint قبل از Migration بررسی شود.
- Price و PriceAdjustment دارای Precision مشخص باشند.
- Query عمومی برای Read در صورت مناسب‌بودن از `AsNoTracking()` استفاده کند.
- Graph فقط به‌اندازهٔ نیاز با `Include` بارگذاری شود.
- Migration دستی بدون بررسی Snapshot ساخته یا ویرایش نشود.
- Repository مسئول دسترسی به داده است، نه Business Rule.

## قواعد API

- قرارداد عمومی از قرارداد Admin جدا بماند.
- API عمومی فقط Category فعال، Product منتشرشده و Option/Value فعال را نمایش دهد.
- Request نامعتبر پاسخ مشخص و قابل‌استفاده برای Frontend داشته باشد.
- اطلاعات داخلی Entity مستقیماً Serialize نشود؛ از Response Contract استفاده شود.
- عملیات Admin پیش از Production باید Authorization داشته باشند.
- خطا و Exception خام به Client نشت نکند.
- CORS بر اساس Environment تنظیم شود.

## تصاویر محصول

- `ProductImage` فعلاً URL، Primary بودن و DisplayOrder را نگهداری می‌کند.
- Upload واقعی فایل هنوز پیاده‌سازی نشده است.
- Storage Provider نباید مستقیماً به Domain وابسته شود.
- فایل Uploadشده باید از نظر حجم، نوع واقعی و نام امن بررسی شود.
- Credential ذخیره‌سازی فقط در Backend نگهداری شود.
- حذف یا شکست Upload نباید فایل یتیم یا رکورد بدون فایل ایجاد کند.
- UI آینده باید Preview، Progress، Error، ترتیب و Primary Image را پوشش دهد.

## Cart، قیمت و Order آینده

- Cart با کلید `shop-cart` در `localStorage` ذخیره می‌شود.
- انتخاب متفاوت Optionها باید Cart Line متفاوت ایجاد کند.
- قیمت واحد برابر قیمت پایه به‌علاوهٔ PriceAdjustment انتخاب‌هاست.
- Client منبع معتبر قیمت نهایی نیست.
- Checkout آینده باید قیمت را در Backend محاسبه کند.
- OrderItem باید Snapshot نام، قیمت و گزینه‌ها را نگهداری کند.
- تغییر Product نباید Order قدیمی را تغییر دهد.

## SSR

فایل‌های اصلی SSR:

- `shop-client/src/main.server.ts`
- `shop-client/src/server.ts`
- `shop-client/src/app/app.config.server.ts`
- `shop-client/src/app/app.routes.server.ts`

- Admin به‌صورت Client-rendered اجرا می‌شود.
- Product و Category فعلاً Prerender هستند.
- پارامترهای Prerender هنوز از Mock می‌آیند.
- هنگام خطای Build، وابستگی API در زمان Prerender بررسی شود.
- Production نباید به `localhost` وابسته باشد.

## قواعد UI

- راهنمای کامل در `docs/UI_GUIDELINES.md` قرار دارد.
- Product Card و Layout موجود بدون دلیل بازطراحی نشوند.
- نمایش گزینه بر اساس `ProductOptionInputType` باشد.
- ColorCode فقط برای Option نوع Color استفاده شود.
- عملیات مخرب Admin از Confirm Dialog استفاده کنند.
- UI نباید موفقیت ثبت سفارش یا پرداخت را پیش از پاسخ Backend نمایش دهد.

## مستندات

| فایل | محتوا |
| --- | --- |
| `docs/PROJECT_CONTEXT.md` | هدف، وضعیت و تصمیم‌های اصلی پروژه |
| `docs/ARCHITECTURE.md` | معماری واقعی Frontend و Backend |
| `docs/UI_GUIDELINES.md` | قواعد رابط، RTL، Theme و Responsive |
| `docs/ROADMAP.md` | ترتیب توسعه و Featureهای آینده |
| `shop-client/README.md` | راه‌اندازی و وضعیت Frontend |

در تغییرات کوچک سند جدید نسازید. تصمیم‌ها و تغییرات مهم را در نزدیک‌ترین سند موجود ثبت کنید.

## چک‌لیست پایان تغییر

- [ ] فقط فایل‌های مرتبط تغییر کرده‌اند.
- [ ] Build مرتبط موفق است یا علت عدم اجرا گزارش شده است.
- [ ] تست مرتبط اجرا شده یا نبود آن مشخص شده است.
- [ ] RTL، Dark Theme، Responsive و SSR در تغییر Frontend بررسی شده‌اند.
- [ ] Business Rule در لایهٔ درست قرار دارد.
- [ ] قرارداد عمومی و Admin از هم جدا مانده‌اند.
- [ ] Secret یا اطلاعات حساس وارد کد نشده است.
- [ ] در صورت تغییر مهم، مستند مرتبط به‌روز شده است.

</div>
