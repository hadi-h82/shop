# راهنمای UI فعلی

این سند فقط الگوها و محدودیت‌های قابل مشاهده در کد فعلی `shop-client/` را ثبت می‌کند.

## زبان و RTL

- تگ ریشهٔ HTML در `shop-client/src/index.html` با `lang="fa"` و `dir="rtl"` تعریف شده است.
- در `shop-client/src/styles.scss`، برای `body` مقدارهای `direction: rtl` و `text-align: right` تنظیم شده‌اند و فونت پیش‌فرض `Vazirmatn, sans-serif` است.
- متن‌های رابط، پیام‌های فرم و `aria-label`های موجود در قالب‌ها فارسی‌اند؛ نمونه‌ها در `src/app/layout/header/header.html`، `src/app/pages/checkout/checkout.html` و قالب‌های `src/app/pages/auth/` قرار دارند.

## جهت‌دهی منطقی CSS

- جهت‌دهی منطقی در بعضی استایل‌ها به‌کار رفته است؛ از جمله `inset-inline-start` در `src/app/layout/header/header.scss` و `inset-inline` و `padding-inline` در `src/app/pages/home/home.scss` و `header.scss`.
- `margin-inline-start: auto` در breakpoint تبلت Header به‌کار رفته است (`src/app/layout/header/header.scss`).
- استفاده یکدست نیست: همان فایل Header برای underline لینک فعال از `right` و `left` و برای نشانِ عدم موجودی Product Card از `right` استفاده می‌کند. این ناهماهنگی در بخش محدودیت‌ها ثبت شده است.

## Responsive و breakpointها

breakpointهای واقعی موجود، وابسته به کامپوننت هستند و یک فایل مرکزی breakpoint وجود ندارد:

| breakpoint / media query | فایل‌های استفاده‌کننده و رفتار قابل مشاهده |
| --- | --- |
| `max-width: 1100px` | `src/app/pages/home/home.scss` |
| `max-width: 1000px` | Header در `layout/header/header.scss`، خانه در `pages/home/home.scss` و صفحهٔ دسته‌بندی در `pages/category-products/category-products.scss` |
| `max-width: 900px` | Cart در `pages/cart/cart.scss` و Checkout در `pages/checkout/checkout.scss`؛ layoutهای چندستونه به تک‌ستونه تغییر می‌کنند |
| `max-width: 800px` | `pages/product-detail/product-detail.scss` |
| `max-width: 768px` | `pages/home/home.scss` |
| `max-width: 760px` | `pages/products/products.scss` |
| `max-width: 700px` | `pages/home/home.scss` |
| `max-width: 600px` | Header، Footer، auth styles، Cart، Checkout، Products و Category Products؛ همچنین Product Card برای صفحه‌های کوچک |
| `max-width: 480px` | `pages/home/home.scss` و `pages/products/products.scss` |
| `max-width: 360px` | Header و Home برای موبایل‌های بسیار کوچک |
| `(hover: none), (max-width: 600px)` | `shared/ui/product-card/product-card.scss`؛ نمایش overlay کارت برای touch و موبایل دائمی می‌شود |
| `(prefers-reduced-motion: reduce)` | `shared/ui/product-card/product-card.scss` و `pages/home/home.scss`؛ transitionها حذف می‌شوند |

## Header و رفتار موبایل

- Header در `src/app/layout/header/header.ts` و `header.html` شامل لوگو، جست‌وجو، ناوبری خانه/محصولات، کنترل تم، سبد خرید و پیوند ورود/ثبت‌نام است.
- استایل آن در `src/app/layout/header/header.scss` sticky است و `z-index: 1000` دارد.
- در `max-width: 1000px`، container Header wrap می‌شود و جست‌وجو به ردیف بعد و عرض کامل منتقل می‌شود.
- در `max-width: 600px`، Header به grid دو ردیفه تبدیل می‌شود: لوگو، تم، سبد و ورود در ردیف اول؛ جست‌وجو با تمام ستون‌ها در ردیف دوم. متن سبد و زیرعنوان لوگو پنهان می‌شوند.
- در `max-width: 600px`، `site-header__nav` با `display: none` پنهان می‌شود.
- **پیاده‌سازی‌نشده:** در قالب و TypeScript Header دکمهٔ بازکردن منوی موبایل، پنل منو، state منو یا جایگزینی برای navigation پنهان‌شده وجود ندارد.
- ورودی جست‌وجو در `header.html` وجود دارد، اما هیچ binding، handler یا منطق جست‌وجو ندارد.

## Light Theme و Dark Theme

- توکن‌های تم روشن در `src/styles/abstracts/_tokens.scss` زیر `:root` و جایگزین‌های تم تیره زیر `html[data-theme='dark']` تعریف شده‌اند.
- اسکریپت آغازین `src/index.html` ابتدا کلید `shop-theme` را از `localStorage` می‌خواند؛ اگر مقدار معتبر وجود نداشته باشد، از `prefers-color-scheme` استفاده و `data-theme` را روی عنصر `html` تنظیم می‌کند.
- `src/app/core/services/theme.ts` در مرورگر تم را از `document.documentElement.dataset.theme` می‌خواند، با `toggle()` تغییر می‌دهد و در `localStorage` با همان کلید `shop-theme` ذخیره می‌کند.
- کنترل تم Header در `src/app/layout/header/header.html` به `themeService.toggle()` متصل است. آیکن‌های روشن/تیره در `header.scss` با `:host-context(html[data-theme='dark'])` تغییر می‌کنند.
- رنگ‌های UI در فایل‌های SCSS عمدتاً از `var(--color-…)` استفاده می‌کنند؛ تغییر بصری باید توکن‌های هر دو تم را در نظر بگیرد.

## توکن‌ها و متغیرهای موجود

تمام توکن‌های زیر در `src/styles/abstracts/_tokens.scss` تعریف شده‌اند:

- رنگ برند: `--color-primary`، `--color-primary-hover` و `--color-on-primary`.
- سطح‌ها: `--color-background`، `--color-surface` و `--color-surface-elevated`.
- متن و مرز: `--color-text`، `--color-muted`، `--color-border` و `--color-border-strong`.
- ورودی‌ها: `--color-input-background`، `--color-input-background-hover`، `--color-input-text`، `--color-input-placeholder`، `--color-input-border` و `--color-input-border-focus`.
- دکمه‌ها: `--color-button-primary`، `--color-button-primary-hover` و `--color-button-primary-text`.
- وضعیت‌ها: `--color-danger` و `--color-success`.
- radius: `--radius-md: 0.75rem`، `--radius-lg: 1rem` و `--radius-xl: 1.5rem`.
- فاصله‌گذاری: `--space-md: 1rem`، `--space-lg: 1.5rem` و `--space-xl: 2rem`.
- عرض container: `--container-width: 1200px`.
- سایه‌ها: `--shadow-sm` و `--shadow-md`.

`--space-sm` در چند فایل مانند `src/app/layout/header/header.scss`، `src/styles/components/_auth.scss` و `src/app/layout/footer/footer.scss` استفاده شده، اما در `_tokens.scss` تعریف نشده است.

## فونت و تایپوگرافی

- `src/styles.scss` فایل فونت `vazirmatn/Vazirmatn-font-face.css` را import می‌کند و `font-family: Vazirmatn, sans-serif` را روی `body` قرار می‌دهد.
- همان فایل typography Angular Material را با `typography: Vazirmatn` در `mat.theme()` پیکربندی می‌کند.
- Product Card در `src/app/shared/ui/product-card/product-card.scss` از `clamp()` برای اندازهٔ عنوان استفاده می‌کند و در حالت touch/mobile اندازه‌ها و line clamp را کاهش می‌دهد.
- Auth card در `src/styles/components/_auth.scss` برای عنوان `h1` وزن `900` و line-height `1.5` دارد؛ اندازهٔ آن در `max-width: 600px` کاهش می‌یابد.

## Angular Material، Material Icons و Tailwind

- Angular Material در `src/styles.scss` با `@use '@angular/material' as mat` و `mat.theme()` استفاده می‌شود؛ پالت‌ها `mat.$orange-palette` و `mat.$red-palette` هستند.
- برای فرم‌ها و دکمه‌های Material، استایل‌های global در `src/styles.scss` وجود دارند: `mat-form-field`، input، placeholder، label، outline، `mat-error`، و دکمه‌های flat/raised/outlined.
- کامپوننت‌ها ماژول‌های Material را به‌صورت local import می‌کنند؛ نمونه‌ها: `MatButtonModule` و `MatIconModule` در `src/app/layout/header/header.ts` و `MatFormFieldModule` و `MatInputModule` در `src/app/pages/checkout/checkout.ts`.
- فونت Material Icons از CDN در `src/index.html` وارد شده و تگ `<mat-icon>` در Header، Product Card، صفحات auth، Cart و Checkout استفاده می‌شود.
- Tailwind با `@import "tailwindcss"` در `src/styles.scss` وارد شده است. استایل اجزای موجود عمدتاً در فایل‌های SCSS همان کامپوننت‌ها و با نام‌گذاری BEM نوشته شده‌اند.

## اجزای مشترک: Product Card

- جزء مشترک موجود `ProductCard` در `src/app/shared/ui/product-card/` است و ورودی الزامی `Product` می‌گیرد.
- قالب `product-card.html` تصویر lazy-loaded، وضعیت عدم موجودی، category، عنوان، قیمت یا قیمت تخفیف‌خورده، پیوند جزئیات و دکمهٔ افزودن به سبد دارد.
- `product-card.scss` از نسبت تصویر `1 / 1`، overlay گرادیانی و ساختار BEM با نام‌های `product-card__…` بهره می‌برد.
- در hover یا `focus-within` روی دسکتاپ، کارت بالا می‌آید، تصویر بزرگ/تیره می‌شود و overlay آشکار می‌گردد.
- در touch یا حداکثر `600px`، overlay همواره نمایش دارد، hover transform حذف می‌شود، عنوان حداکثر دو خط می‌گیرد و کنترل‌ها کوچک‌تر می‌شوند.
- کارت محصول ناموجود کلاس `product-card--unavailable` می‌گیرد، تصویر grayscale می‌شود و دکمهٔ سبد `disabled` است.

## حالت‌های تعاملی و وضعیت‌ها

- **Hover و focus:** Header search از `:focus-within`، Product Card از `:hover` و `:focus-within` و buttonها/پیوندهای متعدد از `:hover` استفاده می‌کنند. `home.scss` حداقل برای کارت‌های دسته‌بندی `:focus-visible` دارد.
- **Disabled:** دکمهٔ سبد Product Card در `product-card.html` برای محصول ناموجود disabled است و در SCSS opacity `0.45` و cursor `not-allowed` دارد.
- **Empty:** حالت خالی در قالب‌ها و استایل‌های صفحات محصولات، دسته‌بندی، Cart و Checkout وجود دارد: `pages/products/products.html`، `pages/category-products/category-products.html`، `pages/cart/cart.html` و `pages/checkout/checkout.html` به‌همراه SCSS متناظر.
- **Error:** خطاهای اعتبارسنجی با `mat-error` در Checkout و فرم‌های auth پیاده‌سازی شده‌اند. `src/styles.scss` رنگ خطای Material را از `--color-danger` می‌گیرد.
- **Loading:** **پیاده‌سازی‌نشده.** در قالب‌ها و کامپوننت‌های UI فعلی loading indicator، skeleton یا state بارگذاری صفحه/داده وجود ندارد.
- **موفقیت:** پیام‌های موفقیت فقط در جریان‌های فراموشی و بازنشانی رمز در قالب‌های `forgot-password.html` و `reset-password.html` وجود دارند؛ ارسال واقعی داده در کد فعلی انجام نمی‌شود.

## فرم‌ها، ورودی‌ها و اعتبارسنجی

- فرم‌های auth در `src/app/pages/auth/` و فرم Checkout در `src/app/pages/checkout/checkout.ts` از Reactive Forms و Angular Material با `appearance="outline"` استفاده می‌کنند.
- اعتبارسنجی‌های فعلی شامل required، minlength، email، الگوی شمارهٔ موبایل و الگوی کد پستی هستند؛ پیام خطا پس از touched شدن control نمایش می‌یابد.
- برای فیلدهای ایمیل، نام و تلفن از `autocomplete` استفاده شده است. فیلد تلفن ثبت‌نام در `register/register.html` دارای `type="tel"` و `inputmode="numeric"` است.
- ورودی‌های رمز عبور در login، register و reset-password دکمهٔ Material برای نمایش/پنهان‌سازی دارند.
- استایل سراسری `src/styles.scss` برای hover، focus، border، placeholder، autofill و `mat-error` فرم‌های Material تعریف شده است.

## دسترس‌پذیری قابل مشاهده

- آیکن‌ها و buttonهای بدون متن در Header، Cart، Product Card، Slider و فرم‌های رمز عبور دارای `aria-label` هستند؛ نمونه‌ها در `layout/header/header.html`، `pages/cart/cart.html`، `shared/ui/product-card/product-card.html` و `pages/home/home.html`.
- اسلایدر خانه در `pages/home/home.html` دارای `aria-label`، کنترل‌های قبلی/بعدی، `aria-hidden` برای اسلاید غیرفعال و مدیریت `tabindex` است.
- Breadcrumb صفحهٔ دسته‌بندی در `pages/category-products/category-products.html` دارای `aria-label="مسیر صفحه"` است و نشانه‌های تزئینی با `aria-hidden` مشخص شده‌اند.
- تصاویر Product Card در `product-card.html` مقدار `alt` را از عنوان محصول می‌گیرند.
- پشتیبانی از `prefers-reduced-motion` در Home و Product Card وجود دارد.
- **پیاده‌سازی‌نشده:** در کد بررسی‌شده، لینک پرش به محتوای اصلی (skip link) یا سازوکار focus management برای navigation موبایل وجود ندارد.

## اصول قابل مشاهده برای موبایل و صفحه‌های کوچک

- در `max-width: 600px`، padding صفحه‌ها معمولاً از `--space-lg` به `--space-md` کاهش می‌یابد؛ نمونه‌ها در `pages/checkout/checkout.scss` و `styles/components/_auth.scss`.
- Cart و Checkout در `max-width: 900px` به تک‌ستونه تغییر می‌کنند؛ Header در `600px` و Product Card در touch/mobile فشرده می‌شوند.
- Footer در `layout/footer/footer.scss` در `600px` ستونی و وسط‌چین می‌شود.
- انیمیشن‌ها در Product Card و Home برای کاربرانی که reduced motion را ترجیح می‌دهند حذف می‌شوند.

## محدودیت‌ها و ناهماهنگی‌های UI قابل مشاهده

- منوی موبایل وجود ندارد: navigation Header در `max-width: 600px` پنهان می‌شود و جایگزینی ندارد.
- جست‌وجوی Header فقط نمایش داده می‌شود و کارکرد جست‌وجو ندارد.
- `--space-sm` استفاده شده ولی در `src/styles/abstracts/_tokens.scss` تعریف نشده است.
- استفاده از propertyهای منطقی و propertyهای فیزیکی چپ/راست هم‌زمان است؛ نمونه‌های فیزیکی در `layout/header/header.scss` و `shared/ui/product-card/product-card.scss` دیده می‌شوند.
- بعضی رنگ‌ها به‌صورت literal در استایل‌های مؤلفه‌ای نوشته شده‌اند، مانند `#fff` و `#8f5833` در `layout/header/header.scss` و `shared/ui/product-card/product-card.scss`، در کنار سیستم توکن‌های رنگی.
- loading state برای صفحات، داده‌ها یا عملیات فرم پیاده‌سازی نشده است.
