import { Component } from '@angular/core';
import { Router } from '@angular/router' 

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent {
  title = 'SteamGamesFront';
  constructor(private router: Router) {}
}
