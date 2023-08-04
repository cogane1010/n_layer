import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, finalize } from 'rxjs/operators';

import { environment } from '@environments/environment';
import { Account, LoginModel, RespondData } from '@app/_models';

const baseUrl = `${environment.apiUrl}/account`;

@Injectable({ providedIn: 'root' })
export class AccountService {
    private respondSubject: BehaviorSubject<RespondData | null>;
    public loginModel:  LoginModel | null; ;
    
    constructor(
        private router: Router,
        private http: HttpClient
    ) {
        this.respondSubject = new BehaviorSubject<RespondData | null>(null);
        this.loginModel = this.respondSubject.value?.data;
    }

    public get accountValue() {
        return this.respondSubject.value?.data;
    }

    login(email: string, password: string) {
        var inputModel = new LoginModel();
        inputModel.userName = email;
        inputModel.password = password;
        return this.http.post<RespondData>(`${baseUrl}/LoginMobile`, inputModel)
            .pipe(map(account => {
                console.log(account)
                this.respondSubject.next(account);
                this.loginModel = this.respondSubject.value?.data;
                this.startRefreshTokenTimer();
                return account;
            }));
    }

    logout() {
        this.http.post<any>(`${baseUrl}/revoke-token`, {}, { withCredentials: true }).subscribe();
        this.stopRefreshTokenTimer();
       // this.accountSubject.next(null);
        this.router.navigate(['/account/login']);
    }

    refreshToken() {
        return this.http.post<any>(`${baseUrl}/refresh-token`, {}, { withCredentials: true })
            .pipe(map((account) => {
              //  this.accountSubject.next(account);
                this.startRefreshTokenTimer();
                return account;
            }));
    }

    register(account: Account) {
        return this.http.post(`${baseUrl}/register`, account);
    }

    verifyEmail(token: string) {
        return this.http.post(`${baseUrl}/verify-email`, { token });
    }

    forgotPassword(email: string) {
        return this.http.post(`${baseUrl}/forgot-password`, { email });
    }

    validateResetToken(token: string) {
        return this.http.post(`${baseUrl}/validate-reset-token`, { token });
    }

    resetPassword(token: string, password: string, confirmPassword: string) {
        return this.http.post(`${baseUrl}/reset-password`, { token, password, confirmPassword });
    }

    getAll() {
        return this.http.get<Account[]>(baseUrl);
    }

    getById(id: string) {
        return this.http.get<Account>(`${baseUrl}/${id}`);
    }

    create(params: any) {
        return this.http.post(baseUrl, params);
    }

    update(id: string, params: any) {
        return this.http.put(`${baseUrl}/${id}`, params)
            .pipe(map((account: any) => {
                // update the current account if it was updated
                // if (account.id === this.accountValue?.id) {
                //     // publish updated account to subscribers
                //     account = { ...this.accountValue, ...account };
                //     this.accountSubject.next(account);
                // }
                return account;
            }));
    }

    delete(id: string) {
        return this.http.delete(`${baseUrl}/${id}`)
            .pipe(finalize(() => {
                // auto logout if the logged in account was deleted
                // if (id === this.accountValue?.id)
                //     this.logout();
            }));
    }

    // helper methods

    private refreshTokenTimeout?: any;

    private startRefreshTokenTimer() {
        // parse json object from base64 encoded jwt token
        //const jwtBase64 = this.accountValue!.jwtToken!.split('.')[1];
        const jwtToken = this.accountValue!.accessToken

        // set a timeout to refresh the token a minute before it expires
        const expires = new Date(this.accountValue!.expiresAt);
        const timeout = expires.getTime() - Date.now() - (60 * 1000);
        this.refreshTokenTimeout = setTimeout(() => this.refreshToken().subscribe(), timeout);
    }

    private stopRefreshTokenTimer() {
        clearTimeout(this.refreshTokenTimeout);
    }
}