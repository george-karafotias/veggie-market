import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductPrice } from './products/product.interface';

@Injectable({
  providedIn: 'root'
})
export class ProductPricesService {
  private apiUrl = 'http://localhost:60347/productPrices';

  constructor(private http: HttpClient) { }

  fetchProductPrices(productId: number, fromDate: string, toDate: string): Observable<ProductPrice[]> {
    return this.http.get<ProductPrice[]>(this.apiUrl + '/' + productId + '/' + fromDate + '/' + toDate);
  }
}
