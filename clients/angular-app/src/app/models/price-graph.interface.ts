export enum PriceGraphCode {
    Category1MinPrice,
    Category1MaxPrice,
    Category2MinPrice,
    Category2MaxPrice,
    ExtraCategory,
    QuantityToSupply,
    DominantPrice
}

export interface PriceGraph {
    name: string,
    code: PriceGraphCode
}

export interface Graph {
    data: any,
    options: any
}