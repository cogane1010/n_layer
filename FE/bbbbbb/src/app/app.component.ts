import { Component } from '@angular/core';

import { AccountService } from './_services';
import { Account, LoginModel, Role } from './_models';

@Component({ selector: 'app-root', templateUrl: 'app.component.html' })
export class AppComponent {
    Role = Role;
    account?: LoginModel | null;

    constructor(private accountService: AccountService) {
       this.account = this.accountService.loginModel;
    }

    logout() {
        this.accountService.logout();
    }
}