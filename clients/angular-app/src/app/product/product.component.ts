import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../product.service';
import { Product, ProductPrice } from '../products/product.interface';
import { ProductPricesService } from '../productPrices.service';
import { PriceGraphPreparationService } from '../price-graph-preparation.service';
import { PriceGraph, PriceGraphCode } from '../models/price-graph.interface';
import { DateHelperService } from '../date-format.service';

@Component({
  selector: 'product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit {

  product: Product | undefined = undefined;
  prices: ProductPrice[] = [];
  fromDate: Date | undefined;
  toDate: Date | undefined;
  availablePriceGraphs: PriceGraph[] = [];
  selectedPriceGraphs: PriceGraph[] = [];
  graphData: any;
  graphOptions: any;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private productPricesService: ProductPricesService,
    private priceGraphPreparationService: PriceGraphPreparationService,
    private dateFormatService: DateHelperService
  ) { }

  ngOnInit() {
    this.initPriceGraphs();
    const productId = Number(this.route.snapshot.paramMap.get('id'));
    this.fetchProduct(productId);
  }

  private initPriceGraphs() {
    this.availablePriceGraphs = [
      {
        code: PriceGraphCode.Category1MinPrice,
        name: 'Category 1 Min Price'
      },
      {
        code: PriceGraphCode.Category1MaxPrice,
        name: 'Category 1 Max Price'
      },
      {
        code: PriceGraphCode.Category2MinPrice,
        name: 'Category 2 Min Price'
      },
      {
        code: PriceGraphCode.Category2MaxPrice,
        name: 'Category 2 Max Price'
      },
      {
        code: PriceGraphCode.ExtraCategory,
        name: 'Extra Category Price'
      },
      {
        code: PriceGraphCode.QuantityToSupply,
        name: 'Quantity to Supply'
      },
      {
        code: PriceGraphCode.DominantPrice,
        name: 'Dominant Price'
      }
    ];
  }

  private resetGraphSelections() {
    this.selectedPriceGraphs = [];
    this.graphData = undefined;
    this.graphOptions = undefined;
  }

  getPrices() {
    setTimeout(() => {
      if (!this.product) return;
      if (this.fromDate === undefined && this.toDate === undefined) return;
      let formattedFromDate = this.dateFormatService.formatDay(this.fromDate);
      let formattedToDate = this.dateFormatService.formatDay(this.toDate);
      this.productPricesService.fetchProductPrices(this.product.ProductId, formattedFromDate, formattedToDate)
        .subscribe(
          (response) => {
            this.prices = response;
            this.resetGraphSelections();
            console.log(response);
          },
          (error) => {
            console.error(error);
          }
        );
    })
  }

  fetchProduct(productId: number) {
    this.productService.fetchProduct(productId)
      .subscribe(
        (response) => {
          this.product = response;
          console.log(response);
        },
        (error) => {
          console.error(error);
        }
      );
  }

  graphSelectionChanged(event: any) {
    let graph = this.priceGraphPreparationService.prepareLineGraph(this.prices, this.selectedPriceGraphs);
    if (graph === undefined) return;
    this.graphData = graph.data;
    this.graphOptions = graph.options;
  }
}
