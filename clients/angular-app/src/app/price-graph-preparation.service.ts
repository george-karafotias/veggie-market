import { Injectable } from "@angular/core";
import { ProductPrice } from "./products/product.interface";
import { Graph, PriceGraph, PriceGraphCode } from "./models/price-graph.interface";

@Injectable({
  providedIn: 'root'
})
export class PriceGraphPreparationService {

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
    const graphOptions = this.prepareLineGraphOptions();
    return {
      data: graphData,
      options: graphOptions
    }
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

  private prepareLineGraphOptions() {
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
}