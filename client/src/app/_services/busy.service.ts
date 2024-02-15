import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
// Exporting a class named BusyService
export class BusyService {

  // Declaring a variable to keep track of busy requests
  busyRequestCount = 0;
  
  constructor(private spinnerService: NgxSpinnerService) { }

  busy(){
    this.busyRequestCount++;

    // Calling the show method of spinner service to display the spinner
    this.spinnerService.show(undefined,{
      type:'line-scale-party',
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333'
    })
  }

  // Defining a method named idle
  idle() {
    // Decrementing the count of busy requests
    this.busyRequestCount--;
    if(this.busyRequestCount <= 0)
    {
      this.busyRequestCount = 0;
      // Resetting the count of busy requests to zero
      this.spinnerService.hide();

      // Calling the hide method of spinner service to hide the spinner
    }
  }
}
