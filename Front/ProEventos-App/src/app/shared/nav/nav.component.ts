import { AccountService } from './../../services/account.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  isCollapsed = true;

  constructor(
    private router: Router,
    public accountService: AccountService
  ) { }

  ngOnInit(): void { }

  logout(): void {
    this.accountService.logout();
    this.router.navigateByUrl('/user/login');
  }

  showMenu(): boolean {
    return this.router.url != '/user/login';
  }

}
