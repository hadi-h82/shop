import { ComponentFixture, TestBed } from '@angular/core/testing';
import {
  ActivatedRoute,
  convertToParamMap,
  provideRouter
} from '@angular/router';
import { BehaviorSubject } from 'rxjs';

import { MOCK_CATEGORIES } from '../../core/mock-data/categories.mock';
import { MOCK_PRODUCTS } from '../../core/mock-data/products.mock';
import { CategoryProducts } from './category-products';

describe('CategoryProducts', () => {
  let component: CategoryProducts;
  let fixture: ComponentFixture<CategoryProducts>;
  const routeParams = new BehaviorSubject(
    convertToParamMap({ slug: 'cake-trays' })
  );

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryProducts],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: routeParams.asObservable(),
            snapshot: {
              paramMap: routeParams.value
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryProducts);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('links back to the categories section on home', () => {
    fixture.detectChanges();

    const backLink: HTMLAnchorElement =
      fixture.nativeElement.querySelector('.category-page__back');

    expect(backLink.getAttribute('href')).toBe('/#popular-categories');
  });

  it('resolves every active category by its existing slug', () => {
    const activeCategories = MOCK_CATEGORIES.filter(
      category => category.isActive
    );

    for (const category of activeCategories) {
      routeParams.next(
        convertToParamMap({ slug: category.slug })
      );

      expect(component.category()?.id).toBe(category.id);
      expect(component.category()?.slug).toBe(category.slug);
      expect(component.guide()?.title).toContain(category.name);
    }
  });

  it('filters products using the resolved category id', () => {
    for (const category of MOCK_CATEGORIES) {
      routeParams.next(
        convertToParamMap({ slug: category.slug })
      );

      const expectedProducts = MOCK_PRODUCTS.filter(
        product => product.categoryId === category.id
      );

      expect(component.products()).toEqual(expectedProducts);
    }
  });

  it('keeps product ids and slugs unique', () => {
    expect(
      new Set(MOCK_PRODUCTS.map(product => product.id)).size
    ).toBe(MOCK_PRODUCTS.length);
    expect(
      new Set(MOCK_PRODUCTS.map(product => product.slug)).size
    ).toBe(MOCK_PRODUCTS.length);
  });

  it('renders the empty state when a category has no products', () => {
    const emptyCategory = MOCK_CATEGORIES.find(
      category =>
        category.isActive &&
        !MOCK_PRODUCTS.some(
          product => product.categoryId === category.id
        )
    );

    expect(emptyCategory).toBeTruthy();

    routeParams.next(
      convertToParamMap({ slug: emptyCategory!.slug })
    );
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain(
      'هنوز محصولی ثبت نشده است'
    );
  });
});
