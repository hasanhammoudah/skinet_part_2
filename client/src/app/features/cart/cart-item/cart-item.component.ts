import { CartService } from './../../../core/services/cart.service';
import { Component, inject, input } from '@angular/core';
import { CartItem } from '../../../shared/models/cart';
import { RouterModule } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-cart-item',
  standalone: true,
  imports: [
    RouterModule,
    MatButton,
    MatIcon,
    CurrencyPipe
  ],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {
item = input.required<CartItem>();
cartService = inject(CartService);

incrementQuantity(){
  this.cartService.addItemToCart(this.item());
}

decrementQuantity(){
  this.cartService.removeItemFromCart(this.item().productId);
}

removeItemFromCart(){
  this.cartService.removeItemFromCart(this.item().productId, this.item().quantity);
}
}
