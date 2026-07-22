import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { MOCK_PRODUCTS } from '../../../core/mock-data/products.mock';
import { ProductCard } from './product-card';

describe('ProductCard', () => {
  let component: ProductCard;
  let fixture: ComponentFixture<ProductCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductCard],
      providers: [provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductCard);
    component = fixture.componentInstance;

    fixture.componentRef.setInput('product', MOCK_PRODUCTS[0]);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('uses the default product image when loading fails', () => {
    const image: HTMLImageElement =
      fixture.nativeElement.querySelector('.product-card__image');

    image.dispatchEvent(new Event('error'));

    expect(image.src).toContain(
      '/images/home/default-product-image.webp'
    );
    expect(image.dataset['fallbackApplied']).toBe('true');
  });
});
