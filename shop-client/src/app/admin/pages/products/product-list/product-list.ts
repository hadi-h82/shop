import { Component } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

interface AdminProductRow {
  id: number;
  name: string;
  category: string;
  price: number;
  status: string;
}

@Component({
  selector: 'app-admin-product-list',

  imports: [
    MatTableModule,
    MatIconModule,
    RouterLink
  ],

  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class AdminProductList {
  readonly displayedColumns = [
    'name',
    'category',
    'price',
    'status',
    'actions'
  ];

  readonly products: AdminProductRow[] = [];
}