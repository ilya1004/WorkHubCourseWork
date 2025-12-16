import {Component} from '@angular/core';
import {UsersComponent} from "./users/users.component";
import {EmployerIndustriesComponent} from "./employer-industries/employer-industries.component";

@Component({
  selector: 'app-identity-service-tools',
  standalone: true,
  imports: [
    UsersComponent,
    EmployerIndustriesComponent,
  ],
  templateUrl: './identity-service-tools.component.html',
  styleUrl: './identity-service-tools.component.scss'
})
export class IdentityServiceToolsComponent {

}
