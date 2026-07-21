# معماری فنی فعلی

این سند تنها بر پایهٔ کد موجود در `shop-client/` تهیه شده است.

## ساختار مخزن

- ریشهٔ مخزن: پوشه‌ای که `AGENTS.md`، `docs/` و `shop-client/` در آن قرار دارند.
- برنامهٔ Angular: `shop-client/`
- پیکربندی Angular و Build: `shop-client/angular.json`
- نقطهٔ ورود مرورگر: `shop-client/src/main.ts`
- نقطهٔ ورود SSR: `shop-client/src/main.server.ts`
- سرور Node/Express: `shop-client/src/server.ts`
- کد برنامه: `shop-client/src/app/`
- فایل‌های استایل سراسری: `shop-client/src/styles.scss` و `shop-client/src/styles/`
- دارایی‌های استاتیک، طبق `angular.json`: `shop-client/public/`

## ساختار `src/app`

| مسیر | مسئولیت فعلی |
| --- | --- |
| `src/app/app.ts`، `app.html` و `app.scss` | کامپوننت ریشهٔ `App` و `router-outlet` اصلی |
| `src/app/app.config.ts` | providerهای مرورگر: Router، client hydration و global error listeners |
| `src/app/app.config.server.ts` | ادغام providerهای مرورگر با providerهای Server Rendering |
| `src/app/app.routes.ts` | تعریف Routeهای کلاینت |
| `src/app/app.routes.server.ts` | تعریف Routeهای رندر سمت‌سرور و prerender |
| `src/app/core/models/` | قراردادهای دادهٔ محصول، دسته‌بندی، گزینه‌های محصول و آیتم سبد |
| `src/app/core/mock-data/` | داده‌های ثابت محصولات و دسته‌بندی‌ها |
| `src/app/core/services/` | سرویس‌های سبد خرید، تم، SEO و یک کلاس `Product` |
| `src/app/layout/` | اجزای مشترک `Header`، `Footer` و `MainLayout` |
| `src/app/pages/` | صفحات خانه، محصولات، دسته‌بندی، جزئیات، سبد، تسویه‌حساب و احراز هویت |
| `src/app/shared/ui/` | جزء UI قابل‌استفادهٔ مجدد `ProductCard` |

## معماری کامپوننت‌ها

تمام کامپوننت‌های فعلی با `@Component` و فهرست `imports` محلی تعریف شده‌اند؛ از جمله `src/app/app.ts`، `src/app/layout/main-layout/main-layout.ts` و `src/app/shared/ui/product-card/product-card.ts`. این ساختار، الگوی standalone components Angular را به‌کار می‌گیرد و هیچ `NgModule` کاربردی در `src/app/` وجود ندارد.

- state در چند بخش با Signals پیاده‌سازی شده است: `signal()` در `src/app/app.ts` و `src/app/core/services/theme.ts`، و `signal()`، `computed()` و `effect()` در `src/app/core/services/cart.ts`.
- `src/app/pages/category-products/category-products.ts` برای پارامتر Route از `toSignal()` استفاده می‌کند.
- `src/app/shared/ui/product-card/product-card.ts` ورودی محصول را با `input.required<Product>()` دریافت می‌کند.
- قالب‌های فعلی از کنترل‌فلو جدید Angular مانند `@if` استفاده می‌کنند؛ نمونه: `src/app/layout/header/header.html` و `src/app/shared/ui/product-card/product-card.html`.

## Routing

Routeهای کلاینت در `src/app/app.routes.ts` تعریف شده‌اند. تمام مسیرهای زیر فرزند Route ریشه با کامپوننت `MainLayout` هستند:

| مسیر | کامپوننت / رفتار |
| --- | --- |
| `/` | `Home` در `src/app/pages/home/home.ts` |
| `/products` | `Products` در `src/app/pages/products/products.ts` |
| `/categories/:slug` | `CategoryProducts` در `src/app/pages/category-products/category-products.ts` |
| `/products/:id` | `ProductDetail` در `src/app/pages/product-detail/product-detail.ts`؛ مقدار `id` با slug محصول تطبیق داده می‌شود |
| `/cart` | `Cart` در `src/app/pages/cart/cart.ts` |
| `/checkout` | `Checkout` در `src/app/pages/checkout/checkout.ts` |
| `/login` | کامپوننت lazy-loaded `Login` |
| `/register` | کامپوننت lazy-loaded `Register` |
| `/forgot-password` | کامپوننت lazy-loaded `ForgotPassword` |
| `/reset-password` | کامپوننت lazy-loaded `ResetPassword` |
| هر مسیر دیگر | redirect به `/` |

### مسیرهای lazy-loaded

چهار مسیر زیر با `loadComponent` در `src/app/app.routes.ts` بارگذاری می‌شوند:

- `/login` ← `src/app/pages/auth/login/login.ts`
- `/register` ← `src/app/pages/auth/register/register.ts`
- `/forgot-password` ← `src/app/pages/auth/forgot-password/forgot-password.ts`
- `/reset-password` ← `src/app/pages/auth/reset-password/reset-password.ts`

## Layout

`src/app/layout/main-layout/main-layout.ts`، `Header` و `Footer` را همراه با `RouterOutlet` import می‌کند. قالب `src/app/layout/main-layout/main-layout.html` به‌ترتیب `<app-header>`، `<main><router-outlet></router-outlet></main>` و `<app-footer>` را رندر می‌کند.

- **Header:** `src/app/layout/header/header.ts` و `header.html` شامل لوگو، ورودی جست‌وجو، پیوندهای خانه و محصولات، کنترل تغییر تم، پیوند سبد خرید با شمارندهٔ `cart.totalItems()` و پیوند ورود/ثبت‌نام است. ورودی جست‌وجو فقط در UI وجود دارد و منطق جست‌وجو ندارد.
- **Footer:** `src/app/layout/footer/footer.ts` و `footer.html` یک فوتر استاتیک با متن فروشگاه و Angular SSR هستند.
- **MainLayout:** `src/app/layout/main-layout/` پوستهٔ مشترک همهٔ صفحات Route شده است.

## مدل‌ها

مدل‌ها در `src/app/core/models/` تعریف شده‌اند:

- `product.model.ts`: رابط `Product` با `id`، `categoryId?`، `title`، `slug`، `price`، `discountPrice?`، `imageUrl`، `categoryName`، `isAvailable` و `options?`.
- `category.model.ts`: رابط `Category` با شناسه، نام، slug، توضیح، تصویر، ترتیب نمایش و وضعیت فعال بودن.
- `cart-item.model.ts`: رابط `CartItem` شامل `product: Product` و `quantity: number`.
- `product-option.model.ts`: نوع `ProductOptionInputType` با مقادیر `select`، `radio` و `color`؛ رابط‌های `ProductOption` و `ProductOptionValue` برای گزینه‌ها، مقادیر، تعدیل قیمت و ترتیب نمایش.

## داده‌های Mock

- محصولات در `src/app/core/mock-data/products.mock.ts` با ثابت `MOCK_PRODUCTS` قرار دارند.
- دسته‌بندی‌ها در `src/app/core/mock-data/categories.mock.ts` با ثابت `MOCK_CATEGORIES` قرار دارند.
- `Home` در `src/app/pages/home/home.ts`، `Products` در `src/app/pages/products/products.ts`، `CategoryProducts` در `src/app/pages/category-products/category-products.ts` و `ProductDetail` در `src/app/pages/product-detail/product-detail.ts` مستقیماً این ثابت‌ها را import می‌کنند.
- `src/app/app.routes.server.ts` نیز همین ثابت‌ها را برای ساخت پارامترهای prerender به‌کار می‌برد.
- در `src/app/core/services/product.ts` کلاسی به نام `Product` وجود دارد، اما خالی است و در این معماری برای دریافت یا مدیریت داده‌های Mock استفاده نمی‌شود.

## سرویس‌ها

| فایل | مسئولیت فعلی |
| --- | --- |
| `src/app/core/services/cart.ts` | نگهداری state سبد خرید، محاسبهٔ تعداد/مبلغ کل، تغییر تعداد و persistence در `localStorage` |
| `src/app/core/services/theme.ts` | نگهداری تم light/dark، تغییر `data-theme` سند و ذخیرهٔ انتخاب در `localStorage` |
| `src/app/core/services/seo.ts` | به‌روزرسانی title، description، تگ‌های Open Graph و canonical link |
| `src/app/core/services/product.ts` | کلاس خالی `Product` با decorator `@Service()`؛ در کد فعلی هیچ مصرف یا قابلیت داده‌ای ندارد |

برای `CartService`، `ThemeService` و `Seo` فایل تست متناظر در همان پوشه با پسوند `.spec.ts` وجود دارد. همچنین برای `Product` یک فایل `product.spec.ts` هست.

## جریان سبد خرید

1. `ProductCard` در `src/app/shared/ui/product-card/product-card.ts` و `ProductDetail` در `src/app/pages/product-detail/product-detail.ts` متد `CartService.add(product)` را فراخوانی می‌کنند.
2. `CartService.add()` در `src/app/core/services/cart.ts` محصول ناموجود یا محصول با قیمت `<= 0` را نمی‌افزاید. برای محصول موجود، `quantity` افزایش می‌یابد؛ وگرنه یک `CartItem` جدید با تعداد ۱ ایجاد می‌شود.
3. state داخلی با `signal<CartItem[]>` نگهداری می‌شود؛ `items` به‌صورت readonly در دسترس است و `totalItems` و `totalPrice` با `computed()` محاسبه می‌شوند.
4. یک `effect()` هر تغییر state را با کلید `shop-cart` در `localStorage` ذخیره می‌کند. state اولیه نیز از همین کلید خوانده می‌شود.
5. دسترسی به `localStorage` با `isPlatformBrowser(PLATFORM_ID)` محافظت شده است تا هنگام SSR استفاده نشود.
6. صفحهٔ `src/app/pages/cart/cart.ts` سرویس را به قالب `cart.html` می‌دهد؛ این قالب عملیات increase، decrease، remove و clear را فراخوانی می‌کند و به `/checkout` پیوند دارد.

## جریان تم Light/Dark

- `src/index.html` دارای `lang="fa"` و `dir="rtl"` است و پیش از bootstrap، کلید `shop-theme` را از `localStorage` می‌خواند. در صورت نبود انتخاب ذخیره‌شده، از `window.matchMedia('(prefers-color-scheme: dark)')` استفاده می‌کند و مقدار `data-theme` عنصر `<html>` را تنظیم می‌کند.
- `src/app/core/services/theme.ts` حالت `ThemeMode` را در Signal نگه می‌دارد. `toggle()` مقدار `data-theme` را بین `light` و `dark` تغییر می‌دهد و در همان کلید `shop-theme` ذخیره می‌کند.
- `src/app/layout/header/header.html` کنترل تغییر تم را به `themeService.toggle()` متصل می‌کند.
- متغیرهای رنگ تم روشن و تیره در `src/styles/abstracts/_tokens.scss`، به‌ترتیب در `:root` و `html[data-theme='dark']` تعریف شده‌اند. `src/styles.scss` توکن‌ها، Angular Material، Tailwind و Vazirmatn را وارد می‌کند.

## SSR و Server Rendering

- در `shop-client/angular.json`، builder برنامه `@angular/build:application` با `outputMode: "server"` تنظیم شده است. نقطه‌های ورود سرور `src/main.server.ts` و `src/server.ts` هستند.
- `src/main.server.ts` تابع bootstrap سرور را با `bootstrapApplication(App, config, context)` صادر می‌کند.
- `src/app/app.config.server.ts` پیکربندی مرورگر (`appConfig`) را با `provideServerRendering(withRoutes(serverRoutes))` ادغام می‌کند.
- `src/server.ts` یک Express app و `AngularNodeAppEngine` می‌سازد، فایل‌های استاتیک browser build را سرو می‌کند و سایر درخواست‌ها را برای رندر Angular به `angularApp.handle(req)` می‌فرستد. پورت از `PORT` یا مقدار پیش‌فرض `4000` خوانده می‌شود.
- `src/app/app.config.ts` در مرورگر Router و `provideClientHydration()` را فراهم می‌کند.

## Prerender

Routeهای سرور در `src/app/app.routes.server.ts` تعریف شده‌اند:

- `products/:id` با `RenderMode.Prerender`: `getPrerenderParams()` برای هر عضو `MOCK_PRODUCTS` یک پارامتر `{ id: product.slug }` تولید می‌کند.
- `categories/:slug` با `RenderMode.Prerender`: `getPrerenderParams()` دسته‌بندی‌های فعال `MOCK_CATEGORIES` را فیلتر می‌کند و برای هرکدام `{ slug: category.slug }` می‌سازد.
- `**` نیز با `RenderMode.Prerender` تعریف شده است.

بنابراین مسیرهای prerender داینامیک فعلاً از داده‌های Mock ایجاد می‌شوند، نه دادهٔ API.

## SEO

- سرویس `src/app/core/services/seo.ts` با `Title`، `Meta` و `DOCUMENT` کار می‌کند.
- متد `Seo.update()` عنوان سند، متای `description`، `og:title`، `og:description`، `og:type`، و در صورت وجود تصویر `og:image` را به‌روز می‌کند.
- برای `canonicalUrl`، همان سرویس `og:url` را به‌روز می‌کند و یک `<link rel="canonical">` را ایجاد یا مقدار `href` آن را تغییر می‌دهد.
- `Home`، `Products`، `CategoryProducts` و `ProductDetail` در فایل‌های TypeScript صفحات متناظر خود این سرویس را فراخوانی می‌کنند.
- URL پایهٔ canonical در این صفحات به شکل ثابت `https://sevart.ir` تعریف شده است.

## محل‌های اتصال آینده به API

در کد فعلی، هیچ فراخوانی HTTP یا اتصال API پیاده‌سازی نشده است. نقاطی که اکنون داده یا خروجی موقت دارند، عبارت‌اند از:

- صفحات `home.ts`، `products.ts`، `category-products.ts` و `product-detail.ts` در `src/app/pages/` که مستقیماً Mockها را می‌خوانند.
- `src/app/app.routes.server.ts` که پارامترهای prerender را از Mockها می‌سازد.
- `src/app/pages/checkout/checkout.ts` که پس از اعتبارسنجی فرم فقط دادهٔ مشتری، اقلام و مبلغ را با `console.log` ثبت می‌کند.
- فایل‌های `src/app/pages/auth/login/login.ts`، `register/register.ts`، `forgot-password/forgot-password.ts` و `reset-password/reset-password.ts` که submit معتبر را فقط با `console.log` ثبت می‌کنند.

## محدودیت‌ها و بدهی‌های فنی قابل مشاهده

- لایهٔ دریافت دادهٔ محصول/دسته‌بندی پیاده‌سازی نشده است؛ صفحات و prerender مستقیماً به Mockها وابسته‌اند.
- `src/app/core/services/product.ts` خالی است و در کد وارد یا استفاده نمی‌شود؛ به‌علاوه نام `Product` آن با رابط `Product` در `src/app/core/models/product.model.ts` هم‌نام است.
- جریان‌های ورود، ثبت‌نام، بازیابی رمز، بازنشانی رمز و تسویه‌حساب فقط اعتبارسنجی سمت کاربر و `console.log` دارند؛ هیچ درخواست شبکه، پاسخ خطا یا تغییر وضعیت موفقیت پیاده‌سازی نشده است.
- سبد خرید فقط محلی است و اعتبارسنجی ساختار دادهٔ JSON خوانده‌شده از `localStorage` به بررسی `Array.isArray` محدود می‌شود.
- `CartService.add()` افزودن محصول با قیمت `0` یا کمتر را رد می‌کند، در حالی که داده‌های Mock فعلی قیمت `0` دارند؛ در نتیجه این محصولات از راه `CartService` قابل افزودن نیستند.
- جست‌وجوی Header در `src/app/layout/header/header.html` فقط یک input است و به داده یا Route وصل نیست.
- URL پایهٔ SEO در صفحات به‌صورت ثابت و تکراری تعریف شده است.
