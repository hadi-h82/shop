import {
  Component,
  inject
} from '@angular/core';

import {
  RouterLink
} from '@angular/router';

import {
  toSignal
} from '@angular/core/rxjs-interop';

import {
  MatIconModule
} from '@angular/material/icon';

import {
  AdminCategoryService
} from '../../../services/admin-category.service';

import {
  Category
} from '../../../../core/models/category.model';


@Component({
  selector: 'app-admin-category-list',

  imports: [
    RouterLink,
    MatIconModule
  ],

  templateUrl: './category-list.html',
  styleUrl: './category-list.scss'
})
export class AdminCategoryList {

  private readonly categoryService =
    inject(AdminCategoryService);

  readonly categories = toSignal(
    this.categoryService.getAll(),
    {
      initialValue: [] as Category[]
    }
  );
}