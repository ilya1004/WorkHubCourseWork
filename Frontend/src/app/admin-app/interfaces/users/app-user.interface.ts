import {FreelancerProfile} from "./freelancer-profile.interface";
import {UserRole} from "./user-role.interface";
import {EmployerProfile} from "./employer-profile.interface";

export interface AppUser {
  id: string;
  registeredAt: string;
  imageUrl: string | null;
  email: string;
  isEmailConfirmed: boolean;
  isActive: boolean;
  freelancerProfile: FreelancerProfile | null;
  employerProfile: EmployerProfile | null;
  role: UserRole | null;
}