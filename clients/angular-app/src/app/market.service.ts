import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Market } from './products/product.interface';

@Injectable({
    providedIn: 'root'
})
export class MarketService {
    private apiUrl = 'http://localhost:60347/markets';

    constructor(private http: HttpClient) { }

    fetchMarkets(): Observable<Market[]> {
        return this.http.get<Market[]>(this.apiUrl);
    }

    fetchMarket(marketName: string): Observable<Market> {
        return this.http.get<Market>(this.apiUrl + '/' + marketName);
    }
}
