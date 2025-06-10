import { Market, Product, ProductPriceType } from "../products/product.interface";

export enum PlotGroup {
    Market = 'Market',
    Product = 'Product',
    Price = 'Price'
}

export interface PriceRetrievalParameters {
    selectedMarkets: Market[];
    selectedProducts: Product[];
    selectedPrices: ProductPriceType[];
    fromDate: Date | undefined;
    toDate: Date | undefined;
}