import { BusyService } from './../../core/services/busy.service';
import { Component, inject } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatBadge } from '@angular/material/badge';
import { MatButton } from '@angular/material/button';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import { CartService } from '../../core/services/cart.service';
import { AccountService } from '../../core/services/account.service';
import { MatMenu, MatMenuItem, MatMenuTrigger } from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';



@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    MatIcon,
    MatButton,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBarModule,
    MatMenuTrigger,
    MatMenu,
    MatDivider,
    MatMenuItem

  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
busyService =inject(BusyService);
cartService = inject(CartService);
accountService = inject(AccountService);
private router = inject(Router); 

logout(){
  this.accountService.logout().subscribe({
    next:()=>{
      this.accountService.currentUser.set(null);
      this.router.navigateByUrl('/');
    }
  });
}

}
