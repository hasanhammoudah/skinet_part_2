import { Route } from "@angular/router";

import { emptyCartGuard } from "../../core/guards/empty-cart.guard";
import { authGuard } from "../../core/guards/auth.guard";
import { LoginComponent } from "./login/login.component";
import { RegisterComponent } from "./register/register.component";


export const accountRoutes: Route[] = 
    [
        {path:'login',component:LoginComponent},
        {path:'register',component:RegisterComponent},
    ];