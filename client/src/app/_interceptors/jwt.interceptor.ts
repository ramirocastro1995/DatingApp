import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account.service';

// Injectable decorator marks it as a service that can be injected into other components/classes.
@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  // Inject AccountService in the constructor
  constructor(private accountService: AccountService) {}

  // Implement the intercept method from HttpInterceptor interface
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    // Subscribe to the currentUser$ observable from accountService
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next : user => {
        // If user is not null, clone the request and set the Authorization header with the user token
        if(user){
          request = request.clone({
            setHeaders :{
              Authorization :  `Bearer ${user.token}` 
            }
          })
        }
      }
    })

    // Pass the cloned request to the next interceptor in the chain
    return next.handle(request);
  }
}

