import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductPrice, ProductPriceType } from './products/product.interface';

@Injectable({
  providedIn: 'root'
})
export class ProductPricesService {
  private apiUrl = 'http://localhost:60347/productPrices';
  private readonly FRONT_SLASH = '/';

  constructor(private http: HttpClient) { }

  fetchProductPrices(productId: number, fromDate: string, toDate: string): Observable<ProductPrice[]> {
    return this.http.get<ProductPrice[]>(this.apiUrl + this.FRONT_SLASH + productId + this.FRONT_SLASH + fromDate + this.FRONT_SLASH + toDate);
  }

  fetchProductMarketPrices(productId: number, marketId: number, fromDate: string, toDate: string): Observable<ProductPrice[]> {
    return this.http.get<ProductPrice[]>(this.apiUrl + this.FRONT_SLASH + marketId + this.FRONT_SLASH + productId + this.FRONT_SLASH + fromDate + this.FRONT_SLASH + toDate);
  }

  fetchProductPriceTypes(): Observable<ProductPriceType[]> {
    return this.http.get<ProductPriceType[]>(this.apiUrl);
  }
}
