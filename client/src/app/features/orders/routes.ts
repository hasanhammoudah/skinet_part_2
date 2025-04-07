import { Route } from "@angular/router";

import { emptyCartGuard } from "../../core/guards/empty-cart.guard";
import { authGuard } from "../../core/guards/auth.guard";
import { OrderComponent } from "./order.component";
import { OrderDetailedComponent } from "./order-detailed/order-detailed.component";

export const orderRoutes: Route[] = 
    [
        {path:'',component:OrderComponent,canActivate:[authGuard]},
        {path:':id',component:OrderDetailedComponent,canActivate:[authGuard]},
    ];