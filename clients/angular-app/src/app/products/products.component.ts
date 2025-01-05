import { Component, OnInit } from '@angular/core';
import { ProductService } from '../product.service';
import { Product } from './product.interface';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {

  products: Product[] = [];

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.fetchProducts();
  }

  fetchProducts() {
    this.productService.fetchProducts()
      .subscribe(
        (response) => {
          this.products = response;
          console.log(this.products);
        },
        (error) => {
          console.error(error);
        }
      );
  }

}
