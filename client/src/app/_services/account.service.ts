import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  // Define base URL from environment variable
  baseUrl = environment.apiUrl;
  
  // Create a new BehaviorSubject that can hold a User object or null
  // BehaviorSubject is a type of subject from RxJS that can hold and emit values
  // It also has the property of storing the “current” value, so new subscribers will get the last emitted value immediately
  private currentUserSource = new BehaviorSubject<User | null>(null);
  
  // Create an Observable from the currentUserSource BehaviorSubject
  // Observables are a primitive type from RxJS that can be used to handle asynchronous data streams
  // In this case, the Observable will emit a new value every time the currentUserSource BehaviorSubject changes
  currentUser$ = this.currentUserSource.asObservable();
  
  constructor(private http: HttpClient) { }

  /**
   * This function is used to authenticate a user.
   * It sends a POST request to the login endpoint with the provided model data.
   * If the user is authenticated successfully, the user data is stored in the local storage
   * and the current user source is updated with the authenticated user's data.
   * 
   * @param model - contains the user's login credentials.
   */
  login(model: any){
    // Send a POST request to the login endpoint with the model data.
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        // Store the response in the user variable.
        const user = response;
        // Check if the user is authenticated successfully.
        if(user){
          // Store the authenticated user data in the local storage.
          localStorage.setItem('user', JSON.stringify(user));
          // Update the current user source with the authenticated user's data.
          this.currentUserSource.next(user);
        }
      })
    )
  }

  // This function is used to register a new user
  // It accepts a model object which contains user details
  register(model:any){
    // It makes a post request to the 'account/register' endpoint with the user details
    // The request returns a User object
    return this.http.post<User>(this.baseUrl+'account/register',model).pipe(
      map(user => {
        // If the user object is not null or undefined
        if(user){
          // The user object is stored in the local storage after converting it to a string
          localStorage.setItem('user',JSON.stringify(user));
          // The user object is also passed to the currentUserSource observable
          this.currentUserSource.next(user);
        }
      })
    );
  }


  setCurrentUser(user: User){
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
