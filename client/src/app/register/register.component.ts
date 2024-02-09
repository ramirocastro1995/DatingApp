import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {}

  constructor(private accountService: AccountService,private toastr: ToastrService){}

  ngOnInit(): void{
  }
  // This function is used to register a new user. 
  register(){
    // We call the register function from the accountService and pass in the model.
    this.accountService.register(this.model).subscribe({
      // If the registration is successful, we call the cancel function.
      next: () => {
        this.cancel();
      },
      // If there is an error during registration, we display an error message using toastr and log the error in the console.
      error: error => {
      this.toastr.error(error.error);
      console.log(error);
    }
    })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
