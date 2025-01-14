import { ShopParams } from './../../shared/models/shopParams';
import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { ProductItemComponent } from "./product-item/product-item.component";
import { MatDialog} from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatCard } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import {MatMenuModule, MatMenuTrigger} from '@angular/material/menu';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';



@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [
    ProductItemComponent,
    MatButton,
    MatIcon,
    MatMenuModule,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger
],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit{
  private shopService = inject(ShopService);
  private dialogService = inject(MatDialog);
  products: Product[] = [];
  sortOptions = [
    {name:'Alphabetical',value:'name'},
    {name:'Price: Low-High',value:'priceAsc'},
    {name:'Price: High-Low',value:'priceDesc'},

  ]
  // constructor(private http:HttpClient){}
  shopParams = new ShopParams();

  ngOnInit(): void {
   this.initializeShop();
  }
  initializeShop(){
    this.shopService.getBrands();
    this.shopService.getTypes();
    this.getProducts();
    
  }
  getProducts(){
    this.shopService.getProducts(this.shopParams).subscribe({
      next:response => this.products = response.data,
      error:error=>console.log(error),
    
    });
  }


  onSortChange(event: MatSelectionListChange){
   const selectedOption = event.options[0];
   if(selectedOption){
    this.shopParams.sort = selectedOption.value;
    //console.log(this.selectedSort);
    this.getProducts();
    
   }
  }
  openFiltersDialogRef(){
   const dialogRef = this.dialogService.open(FiltersDialogComponent,{
    minWidth:'500px',
    data:{
      selectedBrands:this.shopParams.brands,
      selectedTypes: this.shopParams.types
    }
   });
   dialogRef.afterClosed().subscribe({
    next:result=>{
      if(result){
        // console.log(result);
        this.shopParams.brands = result.selectedBrands;
        this.shopParams.types = result.selectedTypes;
        // apply filters
        this.getProducts();

      }
    }
   })
  }
}
