export enum ProductOptionInputType {
  Select = 1,
  Radio = 2,
  Color = 3,
}

export interface ProductOption {
  id: number;

  productOptionDefinitionId: number;

  name: string;

  inputType: ProductOptionInputType;

  isRequired: boolean;

  displayOrder: number;

  isActive: boolean;

  values: ProductOptionValue[];
}

export interface ProductOptionValue {
  id: number;

  label: string;

  value: string;

  priceAdjustment: number;

  colorCode?: string | null;

  isActive: boolean;

  displayOrder: number;
}