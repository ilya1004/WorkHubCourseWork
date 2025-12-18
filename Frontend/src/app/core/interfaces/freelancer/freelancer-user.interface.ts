export interface FreelancerUser {
  id: string;
  nickname: string;
  firstName: string;
  lastName: string;
  about: string;
  email: string;
  registeredAt: string;
  stripeAccountId?: string | null;
  imageUrl: string | null;
  roleName: string;
}