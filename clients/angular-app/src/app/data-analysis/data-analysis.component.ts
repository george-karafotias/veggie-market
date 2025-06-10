import { Component, OnInit } from '@angular/core';
import { Market, Product, ProductPrice, ProductPriceType } from '../products/product.interface';
import { MarketService } from '../market.service';
import { ProductService } from '../product.service';
import { DateFormatService } from '../date-format.service';
import { ProductPricesService } from '../productPrices.service';
import { PlotGroup, PriceRetrievalParameters } from '../models/data-analysis.interface';
import { PriceGraphPreparationService } from '../price-graph-preparation.service';

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
  prices: ProductPrice[] = [];
  selectedPlotGroups: PlotGroup[] = [];
  selectedPlotGroup: PlotGroup | undefined;

  constructor(
    private marketService: MarketService,
    private productService: ProductService,
    private productPricesService: ProductPricesService,
    private dateFormatService: DateFormatService,
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
          console.log(this.markets);
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
          console.log(this.products);
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
          console.log(this.productPriceTypes);
        },
        (error) => {
          console.error(error);
        }
      );
  }

  plot() {
    this.retrievePrices();
    if (!this.prices || this.prices.length === 0) return;
    const priveRetrievalParameters: PriceRetrievalParameters = {
      selectedMarkets: this.selectedMarkets,
      selectedProducts: this.selectedProducts,
      selectedPrices: this.selectedProductPriceTypes,
      fromDate: this.fromDate,
      toDate: this.toDate
    }

    //this.priceGraphPreparationService.prepareGraphs(this.prices, priveRetrievalParameters, this.selectedPlotGroups);
  }

  updatePlotGroups() {
    if (!(this.selectedMarkets.length + this.selectedProducts.length + this.selectedProductPriceTypes.length > 4)) {
      this.selectedPlotGroups = [];
      return;
    }

    if (this.selectedMarkets.length > 1) this.selectedPlotGroups.push(PlotGroup.Market);
    if (this.selectedProducts.length > 1) this.selectedPlotGroups.push(PlotGroup.Product);
    if (this.selectedProductPriceTypes.length > 1) this.selectedPlotGroups.push(PlotGroup.Price);
  }

  private retrievePrices() {
    setTimeout(() => {
      if (!this.selectedMarkets || this.selectedMarkets.length === 0) return;
      if (!this.selectedProducts || this.selectedProducts.length === 0) return;
      if (this.fromDate === undefined && this.toDate === undefined) return;
      let formattedFromDate = this.dateFormatService.formatDay(this.fromDate);
      let formattedToDate = this.dateFormatService.formatDay(this.toDate);

      for (let i = 0; i < this.selectedMarkets.length; i++) {
        const selectedMarket = this.selectedMarkets[i];
        for (let j = 0; j < this.selectedProducts.length; j++) {
          const selectedProduct = this.selectedProducts[j];

          this.productPricesService.fetchProductMarketPrices(selectedMarket.MarketId, selectedProduct.ProductId, formattedFromDate, formattedToDate)
            .subscribe(
              (response) => {
                response.forEach(productPrice => {
                  this.prices.push(productPrice);
                });
                console.log(response);
              },
              (error) => {
                console.error(error);
              }
            );
        }
      }
    })
  }
}
