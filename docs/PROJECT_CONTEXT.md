# زمینهٔ پروژه Sevart

## معرفی

Sevart یک فروشگاه اینترنتی فارسی و راست‌به‌چپ برای فروش تخصصی لوازم قنادی و شیرینی‌پزی است. هدف پروژه ساخت یک فروشگاه واقعی، قابل‌توسعه و آمادهٔ حرکت به سمت محیط Production است؛ نه یک نمونهٔ نمایشی CRUD.

محصولات هدف شامل سینی کیک، سینی فینگر فود، محصولات پلکسی، باکس، استنسیل، جعبه و ابزار و لوازم قنادی هستند.

## هویت و تجربهٔ برند

- نام برند: `Sevart`
- دامنهٔ هدف: `https://sevart.ir`
- زبان رابط کاربری: فارسی
- جهت رابط کاربری: راست‌به‌چپ (`RTL`)
- سبک بصری: مینیمال، مدرن، تمیز، محصول‌محور و Premium
- رنگ‌های اصلی: طیف کرم و قهوه‌ای با استفادهٔ محدود از طلایی
- مرجع الهام طراحی: `bodenow.com`، بدون کپی مستقیم

داستان مفهومی برند بر عشق، علاقه و حرکت برخلاف جریان تأکید دارد. این مفهوم باید در هویت بصری ظریف باقی بماند و به شکل مستقیم یا بیش‌ازحد literal نمایش داده نشود.

## هدف محصول

فروشگاه باید مسیر ساده‌ای برای کشف دسته‌بندی‌ها، مشاهدهٔ محصولات، انتخاب گزینه‌های پویا، محاسبهٔ قیمت نهایی و افزودن محصول به سبد خرید فراهم کند.

اصول اصلی محصول:

- موجودی یا تعداد Stock نباید در Storefront به مشتری نمایش داده شود؛ محصولات می‌توانند پس از ثبت سفارش ساخته شوند.
- دسته‌بندی‌ها و اطلاعات عمومی محصولات باید از Backend و Database دریافت شوند.
- گزینه‌های محصول باید generic و مختص هر محصول باشند و به Color یا Size محدود نشوند.
- قیمت نهایی هر واحد محصول از قیمت پایه به‌علاوهٔ مجموع `PriceAdjustment` گزینه‌های انتخاب‌شده محاسبه می‌شود.
- یک محصول با دو ترکیب متفاوت از گزینه‌ها می‌تواند دو ردیف مستقل در سبد خرید ایجاد کند.
- سفارش‌های آینده باید اطلاعات محصول، قیمت و گزینه‌های انتخاب‌شده را به‌صورت Snapshot نگهداری کنند تا تغییرات بعدی محصول، تاریخچهٔ سفارش را تغییر ندهد.

## ساختار مخزن

مخزن از سه بخش اصلی تشکیل شده است:

| مسیر | مسئولیت |
| --- | --- |
| `shop-client/` | برنامهٔ Angular شامل Storefront و پنل مدیریت |
| `shop-server/` | Backend مبتنی بر ASP.NET Core Web API و Clean Architecture |
| `docs/` | مستندات زمینه، معماری، رابط کاربری و نقشهٔ راه |

## فناوری‌های Frontend

- Angular `22.0.x`
- Angular CLI و Angular Build `22.0.5`
- Standalone Components
- Angular Signals و RxJS
- Reactive Forms
- Angular Material و CDK `22.0.4`
- Angular SSR `22.0.5` با Express `5.1.0`
- TypeScript `~6.0.2`
- SCSS و Tailwind CSS `4.3.2`
- Vazirmatn `33.0.3`
- Vitest `4.0.8`

قواعد اصلی توسعهٔ Frontend:

- استفاده از syntax مدرن Angular شامل `@if`، `@for`، `signal()`، `computed()` و `inject()`
- عدم استفاده از `*ngFor` در توسعه‌های جدید
- حفظ معماری Standalone و lazy loading موجود
- حفظ RTL، رابط فارسی، Responsive Design و Dark Theme
- دسترسی SSR-safe به `window`، `document` و `localStorage`
- استفاده از `PLATFORM_ID` و `isPlatformBrowser` برای APIهای مختص مرورگر
- استفاده از `DOCUMENT` برای تعامل با DOM
- عدم درج کامنت فارسی در کدهای جدید

## فناوری‌های Backend

- C# و .NET
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- OpenAPI و Swagger در محیط Development
- Clean Architecture

Solution در `shop-server/Sevart.slnx` قرار دارد و شامل پروژه‌های زیر است:

| پروژه | مسئولیت |
| --- | --- |
| `Sevart.Domain` | Entityها، Enumها، اعتبارسنجی‌ها و رفتارهای کسب‌وکار |
| `Sevart.Application` | قراردادهای Repository و orchestration سطح Application |
| `Sevart.Infrastructure` | EF Core، DbContext، Configurationها، Repositoryها و Migrationها |
| `Sevart.Api` | Controllerها، قراردادهای HTTP، DI، CORS و پیکربندی اجرای API |

جهت وابستگی موردنظر:

- `Application -> Domain`
- `Infrastructure -> Application + Domain`
- `Api -> Application + Infrastructure`
- `Domain` نباید به `Infrastructure` وابسته باشد.

## مدل دامنهٔ پیاده‌سازی‌شده

Entityهای اصلی فعلی عبارت‌اند از:

- `Category`
- `Product`
- `ProductImage`
- `ProductOptionDefinition`
- `ProductOption`
- `ProductOptionValue`

Entityهای اصلی از `BaseEntity` یا `BaseAuditableEntity` استفاده می‌کنند. رفتارهای کسب‌وکار مانند ایجاد و به‌روزرسانی محصول، انتشار، آرشیو، مدیریت تصاویر و فعال یا غیرفعال‌کردن گزینه‌ها و مقادیر در Domain قرار گرفته‌اند.

### وضعیت محصول

`ProductStatus` شامل مقادیر زیر است:

| مقدار | وضعیت |
| --- | --- |
| `1` | `Draft` |
| `2` | `Published` |
| `3` | `Archived` |

### گزینه‌های عمومی محصول

`ProductOptionDefinition` نوع و عنوان یک گزینهٔ قابل‌استفاده را تعریف می‌کند. هر `Product` می‌تواند گزینه‌های مخصوص خود را از این Definitionها بسازد.

`ProductOptionInputType` شامل موارد زیر است:

| مقدار | نوع ورودی |
| --- | --- |
| `1` | `Select` |
| `2` | `Radio` |
| `3` | `Color` |

هر مقدار گزینه می‌تواند شامل اطلاعات زیر باشد:

- `Label`
- `Value`
- `PriceAdjustment`
- `ColorCode`
- `IsActive`
- `DisplayOrder`

در پاسخ عمومی API فقط گزینه‌ها و مقادیر فعال برگردانده می‌شوند و ترتیب آن‌ها بر اساس `DisplayOrder` است.

## Database و Persistence

`SevartDbContext` و EF Core Configurationهای Entityها در پروژهٔ `Sevart.Infrastructure` قرار دارند. Repositoryهای فعلی برای Category، Product و Product Option Definition پیاده‌سازی شده‌اند.

Migrationهای موجود:

- `InitialCreate`
- `AddProductOptionDefinitions`
- `LinkProductOptionToDefinition`

Database فعلی مدل‌های Category، Product، Product Image، Product Option Definition، Product Option و Product Option Value را پوشش می‌دهد. مدل‌های Customer، Address، Order، Payment، Shipping و Discount هنوز پیاده‌سازی نشده‌اند.

## APIهای فعلی

Controllerهای فعلی:

- `CategoriesController`
- `ProductsController`
- `ProductOptionDefinitionsController`

قابلیت‌های فعلی API شامل موارد زیر است:

- دریافت، ایجاد، ویرایش، حذف، فعال‌سازی و غیرفعال‌سازی دسته‌بندی‌ها
- دریافت دسته‌بندی بر اساس شناسه یا Slug
- دریافت دسته‌بندی همراه محصولات آن
- دریافت، ایجاد، ویرایش، حذف، انتشار و آرشیو محصولات
- دریافت محصول عمومی بر اساس Slug
- دریافت محصولات منتشرشدهٔ یک دسته‌بندی
- مدیریت Product Option Definitionها
- نگهداری تصاویر و گزینه‌های پویا در قراردادهای ایجاد و ویرایش محصول

پاسخ عمومی محصول شامل اطلاعات عمومی محصول، تصویر اصلی و گزینه‌های فعال آن است. فیلدهای مدیریتی نباید به‌صورت خودکار در API عمومی منتشر شوند.

## صفحات و قابلیت‌های Frontend

### Storefront

- صفحهٔ خانه با Hero Slider
- دریافت دسته‌بندی‌های فعال از API
- نمایش محصولات منتخب فعلی از Mock Data
- صفحهٔ تمام محصولات با جست‌وجوی محلی روی Mock Data
- صفحهٔ محصولات دسته‌بندی با دریافت داده از API
- صفحهٔ جزئیات محصول با دریافت محصول بر اساس Slug از API
- نمایش گزینه‌های پویا با ورودی‌های Select، Radio و Color
- انتخاب تعداد و محاسبهٔ قیمت نهایی محصول
- افزودن محصول و گزینه‌های انتخاب‌شده به سبد خرید
- صفحهٔ سبد خرید
- صفحهٔ Checkout با فرم و اعتبارسنجی اولیه
- صفحات ورود، ثبت‌نام، فراموشی رمز و بازنشانی رمز
- SEO پایه شامل Title، Description، Open Graph و Canonical URL
- Light/Dark Theme با ذخیرهٔ انتخاب کاربر

### پنل مدیریت

مسیر `/admin` به‌صورت Client-rendered در تنظیمات SSR تعریف شده است و قابلیت‌های فعلی زیر را دارد:

- Dashboard اولیه
- فهرست، ایجاد و ویرایش دسته‌بندی‌ها
- فعال‌سازی، غیرفعال‌سازی و حذف دسته‌بندی‌ها
- فهرست، ایجاد و ویرایش محصولات
- انتشار، آرشیو و حذف محصولات
- مدیریت تصاویر محصول
- مدیریت گزینه‌ها و مقادیر پویا در فرم محصول

احراز هویت و Authorization پنل مدیریت هنوز پیاده‌سازی نشده است.

## وضعیت اتصال Frontend به داده

پروژه در وضعیت گذار از Mock Data به API قرار دارد:

| بخش | منبع دادهٔ فعلی |
| --- | --- |
| دسته‌بندی‌های صفحهٔ خانه | API |
| محصولات منتخب صفحهٔ خانه | Mock Data |
| صفحهٔ تمام محصولات و جست‌وجو | Mock Data |
| صفحهٔ محصولات دسته‌بندی | API |
| جزئیات محصول | API |
| دسته‌بندی‌ها و محصولات پنل مدیریت | API |
| پارامترهای مسیرهای Prerender | Mock Data |
| سبد خرید | `localStorage` |
| Checkout و Auth | فرم محلی و رفتار نمایشی |

Mockها در `shop-client/src/app/core/mock-data/` هنوز برای بخش‌های بالا و ساخت پارامترهای Prerender استفاده می‌شوند و فعلاً نباید بدون اصلاح وابستگی‌ها حذف شوند.

## سبد خرید و قیمت‌گذاری

سبد خرید در `localStorage` و با کلید `shop-cart` نگهداری می‌شود. دسترسی به Storage با `isPlatformBrowser` محافظت شده است.

هر `CartItem` شامل موارد زیر است:

- شناسهٔ یکتای ترکیب محصول و گزینه‌ها (`cartItemId`)
- محصول
- تعداد
- گزینه‌های انتخاب‌شده
- قیمت نهایی یک واحد

شناسهٔ ردیف سبد خرید از شناسهٔ محصول و شناسهٔ گزینه‌ها و مقادیر انتخاب‌شده ساخته می‌شود. بنابراین دو ترکیب متفاوت از یک محصول در یک ردیف ادغام نمی‌شوند.

فرمول قیمت فعلی:

`Final Unit Price = Base Product Price + Sum(Selected Option PriceAdjustment)`

مدل تخفیف، هزینهٔ ارسال، Coupon و قیمت‌گذاری سمت سرور هنوز نهایی نشده‌اند. پیش از Checkout واقعی، قیمت نهایی باید در Backend دوباره محاسبه و اعتبارسنجی شود و نباید به مقدار ذخیره‌شده در مرورگر اعتماد شود.

## SSR، Prerender و Environment

Angular با `outputMode: "server"` و Express برای SSR پیکربندی شده است. Hydration در Client فعال است.

Render Modeهای فعلی:

- مسیرهای `admin/**`: رندر Client
- مسیرهای `products/:id`: Prerender با پارامترهای Mock
- مسیرهای `categories/:slug`: Prerender با پارامترهای Mock
- سایر مسیرها: Prerender

Frontend از فایل‌های Environment زیر استفاده می‌کند:

- Development API: `http://localhost:5090/api`
- Production API: `https://sevart.ir/api`

صفحات متصل به API ممکن است هنگام Prerender در زمان Build درخواست HTTP ارسال کنند. در نتیجه Build می‌تواند به در دسترس‌بودن API وابسته شود. استراتژی نهایی بین Static Prerender، Runtime SSR و Client Rendering هنوز باید پیش از استقرار Production نهایی شود.

## وضعیت قابلیت‌های آینده

موارد زیر هنوز کامل یا عملیاتی نشده‌اند:

- Authentication و Authorization واقعی
- نقش‌های Admin و Customer
- Customer Profile
- Address Management
- Order و OrderItem
- Snapshot محصول و گزینه‌ها در سفارش
- Checkout سمت سرور
- Payment Gateway
- Shipping Method و Shipping Cost
- Discount و Coupon
- جست‌وجوی متصل به API
- گزارش‌ها و مدیریت سفارش‌ها در Admin
- Logging و Exception Handling سراسری
- Rate Limiting و Production Security Hardening
- File Storage واقعی برای تصاویر
- Monitoring، Backup و فرایند کامل Deployment

## تصمیم‌های باز

- استراتژی نهایی SSR و Prerender برای صفحات وابسته به API
- منبع دادهٔ محصولات صفحهٔ خانه و صفحهٔ تمام محصولات
- مدل Authentication و Authorization
- مدل Customer، Address، Order و Snapshotها
- سیاست پرداخت، ارسال و تخفیف
- محل نگهداری و پردازش تصاویر محصول
- قرارداد استاندارد خطا، Validation و Pagination در API
- تنظیم CORS و Secretها برای محیط Production
- زیرساخت استقرار Frontend، Backend و SQL Server

## روش ادامهٔ توسعه

توسعهٔ پروژه به‌صورت مرحله‌ای انجام می‌شود:

1. در هر مرحله فقط یک فایل یا یک مسئلهٔ مشخص بررسی می‌شود.
2. وضعیت موجود پیش از تغییر از روی کد واقعی بررسی می‌شود.
3. تغییرات اضافی و Refactor نامرتبط انجام نمی‌شود.
4. پس از هر تغییر، Build یا Test مرتبط اجرا می‌شود.
5. مرحلهٔ بعد فقط پس از تأیید نتیجه آغاز می‌شود.
