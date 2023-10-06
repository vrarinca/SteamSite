// login.component.ts
import { Component } from '@angular/core';
import { AuthService } from '../shared/auth.service';
import { Router } from '@angular/router'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})

export class LoginComponent {
  email:string = '';
  password:string = '';

  constructor(private authService: AuthService, private router: Router) {}

  onLogin() {
    this.authService.login(this.email, this.password).subscribe(
      (response: any) => {

        alert('Logged in successfully');

        localStorage.setItem('token', 'valid');  
        
        this.router.navigate(['home'])
      } 
      ,
      (error) => {
        alert('Invalid credentials');
        console.error(error);
      }
    );
  }
}
