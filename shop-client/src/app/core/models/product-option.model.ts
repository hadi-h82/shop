export type ProductOptionInputType =
  | 'select'
  | 'radio'
  | 'color';

export interface ProductOption {
  id: number;
  name: string;
  inputType: ProductOptionInputType;
  isRequired: boolean;
  displayOrder: number;
  values: ProductOptionValue[];
}

export interface ProductOptionValue {
  id: number;
  label: string;
  value: string;
  priceAdjustment: number;
  colorCode?: string;
  isActive: boolean;
  displayOrder: number;
}