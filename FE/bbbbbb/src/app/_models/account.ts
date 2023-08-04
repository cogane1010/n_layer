import { Role } from './role';

export class Account {
    id?: string;
    title?: string;
    firstName?: string;
    lastName?: string;
    email?: string;
    role?: Role;
    jwtToken?: string;
}

export class LoginModel {
    accessToken?: string
    tokenType?: string
    scope?: string
    expiresIn?: string
    userName?: string
    issuedAt?:string
    expiresAt?:string
    accessFailedCount?:string
    otpId?:string
    appVersion?:string
    iosVersion?:string
    isForgotPass?:string
    userRoles?: string[]
    password?:string
    role?: Role
}