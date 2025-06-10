
export interface Product {
    ProductId: number;
    ProductName: string;
    ProductType: ProductType;
    ProductPrices?: ProductPrice[] | null;
}

export interface ProductType {
    ProductTypeId: number;
    ProductTypeName: string;
}

export interface Market {
    MarketId: number;
    MarketName: string;
}

export interface ProductPriceType {
    ProductPriceTypeId: string;
    ProductPriceTypeName: string;
}

export interface ProductPrice {
    Product?: Product;
    ProductDate: number;
    FormattedProductDate: string;
    Market?: Market;
    ExtraCategory: number | null;
    Category1MinPrice: number | null;
    Category1MaxPrice: number | null;
    Category2MinPrice: number | null;
    Category2MaxPrice: number | null;
    QuantityToSupply: number | null;
    DominantPrice: number | null;
    PreviousWeekDominantPrice: number | null;
    PreviousYearDominantPrice: number | null;
    PreviousWeekPriceDifference: number | null;
    PreviousYearPriceDifference: number | null;
    SoldQuantityPercentage: number | null;
}