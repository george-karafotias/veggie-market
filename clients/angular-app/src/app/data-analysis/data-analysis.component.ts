import { Component, OnInit } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Market, Product, ProductPrice, ProductPriceType } from '../products/product.interface';
import { MarketService } from '../market.service';
import { ProductService } from '../product.service';
import { DateHelperService } from '../date-helper.service';
import { ProductPricesService } from '../productPrices.service';
import { PlotGroup, PriceRetrievalParameters } from '../models/data-analysis.interface';
import { PriceGraphPreparationService } from '../price-graph-preparation.service';
import { Graph } from '../models/price-graph.interface';

@Component({
  selector: 'data-analysis',
  templateUrl: './data-analysis.component.html',
  styleUrls: ['./data-analysis.component.css']
})
export class DataAnalysisComponent implements OnInit {

  markets: Market[] = [];
  selectedMarkets: Market[] = [];
  products: Product[] = [];
  selectedProducts: Product[] = [];
  fromDate: Date | undefined;
  toDate: Date | undefined;
  productPriceTypes: ProductPriceType[] = [];
  selectedProductPriceTypes: ProductPriceType[] = [];
  selectedPlotGroups: PlotGroup[] = [];
  selectedPlotGroup: PlotGroup | undefined;
  prices: ProductPrice[] = [];
  graphs: Graph[] | undefined = undefined;

  constructor(
    private marketService: MarketService,
    private productService: ProductService,
    private productPricesService: ProductPricesService,
    private dateFormatService: DateHelperService,
    private priceGraphPreparationService: PriceGraphPreparationService
  ) { }

  ngOnInit(): void {
    this.fetchMarkets();
    this.fetchProducts();
    this.fetchProductPriceTypes();
  }

  fetchMarkets() {
    this.marketService.fetchMarkets()
      .subscribe(
        (response) => {
          this.markets = response;
        },
        (error) => {
          console.error(error);
        }
      );
  }

  fetchProducts() {
    this.productService.fetchProducts()
      .subscribe(
        (response) => {
          this.products = response;
        },
        (error) => {
          console.error(error);
        }
      );
  }

  fetchProductPriceTypes() {
    this.productPricesService.fetchProductPriceTypes()
      .subscribe(
        (response) => {
          this.productPriceTypes = response;
        },
        (error) => {
          console.error(error);
        }
      );
  }

  updatePlotGroups() {
    if (!(this.selectedMarkets.length + this.selectedProducts.length + this.selectedProductPriceTypes.length > 4)) {
      this.selectedPlotGroups = [];
      this.selectedPlotGroup = undefined;
      return;
    }

    this.selectedPlotGroups = [];
    if (this.selectedMarkets.length > 1) this.selectedPlotGroups.push(PlotGroup.Market);
    if (this.selectedProducts.length > 1) this.selectedPlotGroups.push(PlotGroup.Product);
    if (this.selectedProductPriceTypes.length > 1) this.selectedPlotGroups.push(PlotGroup.Price);
  }

  async retrievePrices(): Promise<void> {
    if (!this.selectedMarkets || this.selectedMarkets.length === 0) return;
    if (!this.selectedProducts || this.selectedProducts.length === 0) return;
    if (this.fromDate === undefined && this.toDate === undefined) return;

    this.prices = [];
    let formattedFromDate = this.dateFormatService.formatDay(this.fromDate);
    let formattedToDate = this.dateFormatService.formatDay(this.toDate);

    for (let i = 0; i < this.selectedMarkets.length; i++) {
      const selectedMarket = this.selectedMarkets[i];

      for (let j = 0; j < this.selectedProducts.length; j++) {
        const selectedProduct = this.selectedProducts[j];

        try {
          // Convert Observable to Promise and await response
          const response = await firstValueFrom(
            this.productPricesService.fetchProductMarketPrices(
              selectedMarket.MarketId, selectedProduct.ProductId, formattedFromDate, formattedToDate
            )
          );

          response.forEach(productPrice => {
            this.prices.push(productPrice);
          });

        } catch (error) {
          console.error("Error fetching prices:", error);
        }
      }
    }
  }

  async retrievePricesAndPlot() {
    const priveRetrievalParameters: PriceRetrievalParameters = {
      selectedMarkets: this.selectedMarkets,
      selectedProducts: this.selectedProducts,
      selectedPrices: this.selectedProductPriceTypes,
      fromDate: this.fromDate,
      toDate: this.toDate
    }

    await this.retrievePrices();

    this.graphs = this.priceGraphPreparationService.prepareLineGraphs(this.prices, priveRetrievalParameters, this.selectedPlotGroup);
  }
}
