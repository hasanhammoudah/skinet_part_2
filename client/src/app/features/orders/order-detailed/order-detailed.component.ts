import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from "../../../shared/pipes/address.pipe";
import { CardPipe } from "../../../shared/pipes/card.pipe";

@Component({
  selector: 'app-order-detailed',
  standalone: true,
  imports: [
    MatCardModule,
    MatButton,
    DatePipe,
    CurrencyPipe,
    AddressPipe,
    CardPipe,
    RouterLink
  ],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss'
})
export class OrderDetailedComponent implements OnInit {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);
  order?: Order;

  ngOnInit(): void {
    this.loadOrder();
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;

    this.orderService.getOrderDetailed(+id).subscribe({
      next: order => this.order = order
    });
  }

  get shippingPrice(): number {
    return this.order?.deliveryMethod?.price ?? 0;
  }

  get discount(): number {
    if (!this.order?.coupon) return 0;

    const subtotal = this.order.subtotal;

    if (this.order.coupon.amountOff) {
      return this.order.coupon.amountOff;
    }

    if (this.order.coupon.percentOff) {
      return subtotal * (this.order.coupon.percentOff / 100);
    }

    return 0;
  }

  get total(): number {
    return (
      (this.order?.subtotal ?? 0) +
      this.shippingPrice -
      (this.order?.discount ?? 0)
    );
  }
  
}
