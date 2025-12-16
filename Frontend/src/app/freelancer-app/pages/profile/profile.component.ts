import {Component, OnInit} from '@angular/core';
import {ProfileService} from '../../services/profile.service';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {DatePipe, NgIf} from '@angular/common';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {NzSpinComponent} from "ng-zorro-antd/spin";
import {FreelancerUser} from "../../../core/interfaces/freelancer/freelancer-user.interface";
import {NzMessageService} from "ng-zorro-antd/message";
import {NzModalService} from "ng-zorro-antd/modal";
import {Router, RouterModule} from "@angular/router";
import {UsersService} from "../../../core/services/users/users.service";

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    NzFlexDirective,
    NzCardComponent,
    NzButtonComponent,
    NzIconDirective,
    NzInputDirective,
    NzSpaceComponent,
    NzSpaceItemDirective,
    NgIf,
    NzSpinComponent,
    ReactiveFormsModule,
    DatePipe,
    NzInputGroupComponent,
    RouterModule
  ],
  providers: [NzMessageService, NzModalService],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  isEditing: boolean = false;
  isChangingPassword: boolean = false;
  isLoadingUserData: boolean = true;
  isUpdating: boolean = false;
  isChangingPasswordInProgress: boolean = false;

  currentPasswordVisible: boolean = false;
  newPasswordVisible: boolean = false;
  confirmPasswordVisible: boolean = false;

  userData: FreelancerUser = {
    id: '',
    userName: '',
    firstName: '',
    lastName: '',
    about: '',
    email: '',
    registeredAt: '',
    stripeAccountId: null,
    imageUrl: null,
    roleName: ''
  };

  editFreelancerForm = new FormGroup({
    firstName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)]
    }),
    lastName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)]
    }),
    about: new FormControl('', {
      nonNullable: true,
      validators: [Validators.maxLength(1000)]
    }),
    resetImage: new FormControl(false, {
      nonNullable: true
    }),
    image: new FormControl<File | null>(null),
  });

  changePasswordForm = new FormGroup({
    currentPassword: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
    newPassword: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
    confirmNewPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  }, { validators: this.passwordsMatchValidator });

  constructor(
    private profileService: ProfileService,
    private userService: UsersService,
    private message: NzMessageService,
    private modal: NzModalService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    this.isLoadingUserData = true;
    this.profileService.getUserData().subscribe({
      next: (value) => {
        this.userData = value;
        this.isLoadingUserData = false;
      },
      error: (err) => {
        console.error('Error loading user data:', err);
        this.message.error('Failed to load user data', { nzDuration: 3000 });
        this.isLoadingUserData = false;
      }
    });
  }

  onFileSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      const file = fileInput.files[0];
      this.editFreelancerForm.patchValue({ image: file });
    }
  }

  onSubmitEditForm(): void {
    if (this.editFreelancerForm.valid) {
      this.isUpdating = true;
      const formData = new FormData();
      const formValue = this.editFreelancerForm.getRawValue();

      formData.append('FreelancerProfile.FirstName', formValue.firstName);
      formData.append('FreelancerProfile.LastName', formValue.lastName);
      formData.append('FreelancerProfile.About', formValue.about);
      formData.append('FreelancerProfile.ResetImage', String(formValue.resetImage));
      if (formValue.image) {
        formData.append('ImageFile', formValue.image);
      }

      this.profileService.updateFreelancerProfile(formData).subscribe({
        next: () => {
          this.isUpdating = false;
          this.message.success('Profile updated successfully!', { nzDuration: 3000 });
          this.loadUserData();
          this.isEditing = false;
        },
        error: (err) => {
          console.error('Error updating profile:', err);
          this.message.error('Failed to update profile', { nzDuration: 3000 });
          this.isUpdating = false;
        }
      });
    }
  }

  onClickEdit(): void {
    this.isEditing = !this.isEditing;
    this.isChangingPassword = false;
    if (this.isEditing) {
      this.editFreelancerForm.patchValue({
        firstName: this.userData.firstName,
        lastName: this.userData.lastName,
        about: this.userData.about,
      });
    }
  }

  onCancelEdit(): void {
    this.isEditing = false;
    this.editFreelancerForm.reset({
      firstName: this.userData.firstName,
      lastName: this.userData.lastName,
      about: this.userData.about,
      resetImage: false,
      image: null
    });
  }

  onImageError(event: Event): void {
    (event.target as HTMLImageElement).src = '/assets/images/avatar-placeholder.png';
  }

  onClickChangePassword(): void {
    this.isChangingPassword = !this.isChangingPassword;
    this.isEditing = false;
    if (!this.isChangingPassword) {
      this.changePasswordForm.reset();
      this.currentPasswordVisible = false;
      this.newPasswordVisible = false;
      this.confirmPasswordVisible = false;
    }
  }

  onSubmitChangePassword(): void {
    if (this.changePasswordForm.valid) {
      this.isChangingPasswordInProgress = true;
      const formValue = this.changePasswordForm.getRawValue();

      const request = {
        email: this.userData.email!,
        currentPassword: formValue.currentPassword,
        newPassword: formValue.newPassword
      };

      this.profileService.changePassword(request).subscribe({
        next: () => {
          this.isChangingPasswordInProgress = false;
          this.message.success('Password changed successfully!', { nzDuration: 3000 });
          this.isChangingPassword = false;
          this.changePasswordForm.reset();
        },
        error: (err) => {
          this.isChangingPasswordInProgress = false;
          this.message.error(err.error?.message || 'Failed to change password', { nzDuration: 3000 });
          console.error('Error changing password:', err);
        }
      });
    }
  }

  onClickDeleteAccount(): void {
    this.modal.confirm({
      nzTitle: 'Delete Account',
      nzContent: 'Are you sure you want to delete your account? This action cannot be undone.',
      nzOkText: 'Delete',
      nzCancelText: 'Cancel',
      nzOnOk: () => this.deleteAccount()
    });
  }

  private deleteAccount(): void {
    this.userService.deleteUser(this.userData.id).subscribe({
      next: () => {
        this.message.success('Account deleted successfully', { nzDuration: 3000 });
        localStorage.removeItem('jwt_token');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Error deleting account:', err);
        this.message.error(err.error?.message || 'Failed to delete account', { nzDuration: 3000 });
      }
    });
  }

  private passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmNewPassword')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }
}