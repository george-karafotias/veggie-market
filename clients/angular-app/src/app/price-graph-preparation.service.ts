import { Injectable } from "@angular/core";
import { Market, Product, ProductPrice } from "./products/product.interface";
import { Graph, LineSeries, PriceGraph, PriceGraphCode } from "./models/price-graph.interface";
import { PlotGroup, PriceRetrievalParameters } from "./models/data-analysis.interface";
import { DateHelperService } from "./date-helper.service";

@Injectable({
  providedIn: 'root'
})
export class PriceGraphPreparationService {

  constructor(private dateHelperService: DateHelperService) { }

  public prepareLineGraphs(productPrices: ProductPrice[], priceRetrievalParameters: PriceRetrievalParameters, groupBy: PlotGroup | undefined): Graph[] {
    if (!productPrices || !priceRetrievalParameters.selectedPrices) return [];

    if (groupBy === PlotGroup.Market) {
      return this.groupGraphsByMarket(productPrices, priceRetrievalParameters);
    } else if (groupBy === PlotGroup.Product) {
      return this.groupGraphsByProduct(productPrices, priceRetrievalParameters);
    } else if (groupBy === PlotGroup.Price) {
      return this.groupGraphsByPrice(productPrices, priceRetrievalParameters);
    } else {
      let graphs = [];
      graphs.push(this.createLineGraph(priceRetrievalParameters, this.createLineSeriesCollection(productPrices, priceRetrievalParameters), 'Chart'));
      return graphs;
    }
  }

  public prepareLineGraph(productPrices: ProductPrice[], selectedPriceGraphs: PriceGraph[]): Graph | undefined {
    if (!productPrices || !selectedPriceGraphs) return undefined;
    productPrices = this.sortProductPricesInAscedingOrder(productPrices);
    let labels = [];
    for (let i = 0; i < productPrices.length; i++) {
      labels.push(productPrices[i].FormattedProductDate);
    }

    let datasets = [];
    for (let i = 0; i < selectedPriceGraphs.length; i++) {
      const currentPriceGraph = selectedPriceGraphs[i];
      let datasetData = [];
      for (let j = 0; j < productPrices.length; j++) {
        const currentPrice = this.getPrice(productPrices[j], currentPriceGraph);
        if (currentPrice === undefined || currentPrice === null) return;
        datasetData.push(currentPrice);
      }

      datasets.push({
        label: currentPriceGraph.name,
        data: datasetData,
        fill: false,
        borderColor: this.getRandomColor(),
        tension: .4
      });
    }

    const graphData = {
      labels: labels,
      datasets: datasets
    };
    const graphOptions = this.createLineGraphOptions();
    return {
      data: graphData,
      options: graphOptions
    }
  }

  private createHorizontalAxisDateLabels(priceRetrievalParameters: PriceRetrievalParameters): string[] {
    let labels: string[] = [];
    let currentDate = new Date(this.ensureDate(priceRetrievalParameters.fromDate));
    const endDate = this.ensureDate(priceRetrievalParameters.toDate);
    while (currentDate <= endDate) {
      labels.push(this.dateHelperService.formatDay(currentDate));
      currentDate.setDate(currentDate.getDate() + 1);
    }
    return labels;
  }

  private groupGraphsByMarket(productPrices: ProductPrice[], priceRetrievalParameters: PriceRetrievalParameters): Graph[] {
    const selectedYears: number[] = this.dateHelperService.calculateFullYearsList(priceRetrievalParameters.fromDate, priceRetrievalParameters.toDate);

    let graphs: Graph[] = [];
    priceRetrievalParameters.selectedMarkets.forEach(market => {
      let seriesCollection: LineSeries[] = [];

      selectedYears.forEach(year => {
        priceRetrievalParameters.selectedProducts.forEach(product => {
          priceRetrievalParameters.selectedPrices.forEach(priceType => {
            let seriesTitle = product.ProductName + ' ' + year + ' - ' + priceType.ProductPriceTypeName;
            const filteredPrices: ProductPrice[] = this.filterPricesByMarketProductAndYear(productPrices, market, product, year);
            seriesCollection.push(this.createLineSeries(seriesTitle, priceType.ProductPriceTypeId, filteredPrices));
          })
        })
      })

      graphs.push(this.createLineGraph(priceRetrievalParameters, seriesCollection, market.MarketName));
    });

    return graphs;
  }

  private groupGraphsByProduct(productPrices: ProductPrice[], priceRetrievalParameters: PriceRetrievalParameters): Graph[] {
    const selectedYears: number[] = this.dateHelperService.calculateFullYearsList(priceRetrievalParameters.fromDate, priceRetrievalParameters.toDate);

    let graphs: Graph[] = [];
    priceRetrievalParameters.selectedProducts.forEach(product => {
      let seriesCollection: LineSeries[] = [];

      selectedYears.forEach(year => {
        priceRetrievalParameters.selectedMarkets.forEach(market => {
          priceRetrievalParameters.selectedPrices.forEach(priceType => {
            let seriesTitle = market.MarketName + ' ' + year + ' - ' + priceType.ProductPriceTypeName;
            const filteredPrices: ProductPrice[] = this.filterPricesByMarketProductAndYear(productPrices, market, product, year);
            seriesCollection.push(this.createLineSeries(seriesTitle, priceType.ProductPriceTypeId, filteredPrices));
          })
        })
      })

      graphs.push(this.createLineGraph(priceRetrievalParameters, seriesCollection, product.ProductName));
    });

    return graphs;
  }

  private groupGraphsByPrice(productPrices: ProductPrice[], priceRetrievalParameters: PriceRetrievalParameters): Graph[] {
    const selectedYears: number[] = this.dateHelperService.calculateFullYearsList(priceRetrievalParameters.fromDate, priceRetrievalParameters.toDate);

    let graphs: Graph[] = [];
    priceRetrievalParameters.selectedPrices.forEach(priceType => {
      let seriesCollection: LineSeries[] = [];

      selectedYears.forEach(year => {
        priceRetrievalParameters.selectedMarkets.forEach(market => {
          priceRetrievalParameters.selectedProducts.forEach(product => {
            let seriesTitle = market.MarketName + ' ' + year + ' - ' + product.ProductName;
            const filteredPrices: ProductPrice[] = this.filterPricesByMarketProductAndYear(productPrices, market, product, year);
            seriesCollection.push(this.createLineSeries(seriesTitle, priceType.ProductPriceTypeId, filteredPrices));
          })
        })
      })

      graphs.push(this.createLineGraph(priceRetrievalParameters, seriesCollection, priceType.ProductPriceTypeName));
    });

    return graphs;
  }

  private ensureDate(date: Date | undefined): Date {
    return date ?? new Date();
  }

  private createLineGraph(priceRetrievalParameters: PriceRetrievalParameters, lineSeriesCollection: LineSeries[], title: string): Graph {
    const graphData = {
      labels: this.createHorizontalAxisDateLabels(priceRetrievalParameters),
      datasets: lineSeriesCollection
    };
    return {
      data: graphData,
      options: this.createLineGraphOptions()
    };
  }

  private createLineSeriesCollection(productPrices: ProductPrice[], priceRetrievalParameters: PriceRetrievalParameters): LineSeries[] {
    let lineSeriesCollection: LineSeries[] = [];

    priceRetrievalParameters.selectedMarkets.forEach(market => {
      priceRetrievalParameters.selectedProducts.forEach(product => {
        priceRetrievalParameters.selectedPrices.forEach(price => {
          const seriesTitle = market.MarketName + ' - ' + product.ProductName + ' - ' + price.ProductPriceTypeName;
          const filteredProductPrices: ProductPrice[] = this.filterPricesByMarketAndProduct(productPrices, market, product);
          lineSeriesCollection.push(this.createLineSeries(seriesTitle, price.ProductPriceTypeId, filteredProductPrices));
        })
      });
    });

    return lineSeriesCollection;
  }

  private createLineSeries(title: string, priceType: string, prices: ProductPrice[]): LineSeries {
    let lineSeriesData: number[] = [];
    for (let i = 0; i < prices.length; i++) {
      let price = prices[i][priceType as keyof ProductPrice];
      if (price !== null && price !== undefined) {
        lineSeriesData.push(Number(price));
      }
    }

    return {
      label: title,
      data: lineSeriesData,
      fill: false,
      borderColor: this.getRandomColor(),
      tension: .4
    };
  }

  private filterPricesByMarketAndProduct(productPrices: ProductPrice[], market: Market, product: Product): ProductPrice[] {
    let filteredProductPrices: ProductPrice[] = [];
    productPrices.forEach(productPrice => {
      if (productPrice.Market?.MarketId === market.MarketId && productPrice.Product?.ProductId === product.ProductId) {
        filteredProductPrices.push(productPrice);
      }
    });
    return filteredProductPrices;
  }

  private filterPricesByMarketProductAndYear(productPrices: ProductPrice[], market: Market, product: Product, year: number): ProductPrice[] {
    let filteredProductPrices: ProductPrice[] = [];
    productPrices.forEach(productPrice => {
      const priceYear = this.extractYearFromFormattedDate(productPrice.FormattedProductDate);
      if (productPrice.Market?.MarketId === market.MarketId && productPrice.Product?.ProductId === product.ProductId && priceYear === year) {
        filteredProductPrices.push(productPrice);
      }
    });
    return filteredProductPrices;
  }

  private sortProductPricesInAscedingOrder(productPrices: ProductPrice[]): ProductPrice[] {
    if (productPrices.length < 2) return productPrices;
    if (productPrices[1].ProductDate > productPrices[0].ProductDate) return productPrices;
    let sortedProductPrices = [];
    for (let i = productPrices.length - 1; i >= 0; i--) {
      sortedProductPrices.push(productPrices[i]);
    }
    return sortedProductPrices;
  }

  private getRandomColor(): string {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

  private getPrice(productPrice: ProductPrice, selectedPriceGraph: PriceGraph) {
    let graphMap = new Map<PriceGraphCode, any>();
    graphMap.set(PriceGraphCode.Category1MinPrice, productPrice.Category1MinPrice);
    graphMap.set(PriceGraphCode.Category1MaxPrice, productPrice.Category1MaxPrice);
    graphMap.set(PriceGraphCode.Category2MinPrice, productPrice.Category2MinPrice);
    graphMap.set(PriceGraphCode.Category2MaxPrice, productPrice.Category2MaxPrice);
    graphMap.set(PriceGraphCode.DominantPrice, productPrice.DominantPrice);
    graphMap.set(PriceGraphCode.ExtraCategory, productPrice.ExtraCategory);
    graphMap.set(PriceGraphCode.QuantityToSupply, productPrice.QuantityToSupply);

    return graphMap.get(selectedPriceGraph.code);
  }

  private createLineGraphOptions() {
    return {
      responsive: true,
      maintainAspectRatio: true,
      plugins: {
        legend: {
          labels: {
            color: '#495057'
          }
        }
      },
      scales: {
        x: {
          ticks: {
            color: '#495057'
          },
          grid: {
            color: '#ebedef'
          }
        },
        y: {
          ticks: {
            color: '#495057'
          },
          grid: {
            color: '#ebedef'
          }
        }
      }
    };
  }

  private extractYearFromFormattedDate(formattedDate: string): number {
    return Number(formattedDate.substring(formattedDate.length - 4));
  }
}