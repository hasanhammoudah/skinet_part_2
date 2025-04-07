import { Route } from "@angular/router";
import { CheckoutComponent } from "./checkout.component";
import { CheckoutSuccessComponent } from "./checkout-success/checkout-success.component";
import { emptyCartGuard } from "../../core/guards/empty-cart.guard";
import { authGuard } from "../../core/guards/auth.guard";

export const checkoutRoutes: Route[] = 
    [
    {path:'',component:CheckoutComponent,canActivate:[authGuard,emptyCartGuard]},
    {path:'success',component:CheckoutSuccessComponent,canActivate:[authGuard]},
    ];