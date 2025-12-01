import { Injectable } from '@angular/core';
import { delay, Observable, of } from "rxjs";
import { FreelancerUser } from "../../core/interfaces/freelancer/freelancer-user.interface";
import { PaginatedResult } from "../../core/interfaces/common/paginated-result.interface";
import { FreelancerSkill } from "../../core/interfaces/freelancer/freelancer-skill.interface";

@Injectable({
  providedIn: 'root'
})
export class ProfileServiceMockService {
  getUserData(): Observable<FreelancerUser> {
    return of({
      id: 'usr_7a9f3e2c',
      userName: 'ilya_dev',
      firstName: 'Илья',
      lastName: 'Рабец',
      about: 'Full-stack разработчик с 3-летним опытом. Специализация — .NET + Angular. Люблю сложные архитектуры и чистый код.',
      email: 'ilya.rabets@example.com',
      registeredAt: '2023-04-15T10:30:00Z',
      stripeAccountId: 'acct_1N9kLm2eZvKYlo2C',
      imageUrl: 'https://randomuser.me/api/portraits/men/32.jpg',
      roleName: 'Freelancer',
      skills: [
        {id: 'sk_1', name: 'C# / .NET'},
        {id: 'sk_2', name: 'Angular'},
        {id: 'sk_3', name: 'PostgreSQL'},
        {id: 'sk_4', name: 'Docker & Kubernetes'},
        {id: 'sk_5', name: 'Microservices'}
      ]
    } as FreelancerUser).pipe(delay(300));
  }

  getAvailableSkill(): Observable<PaginatedResult<FreelancerSkill>> {
    return of({
      items: [
        {id: 'sk_1', name: 'C# / .NET'},
        {id: 'sk_2', name: 'Angular'},
        {id: 'sk_3', name: 'PostgreSQL'},
        {id: 'sk_4', name: 'Docker & Kubernetes'},
        {id: 'sk_5', name: 'Microservices'},
        {id: 'sk_6', name: 'React'},
        {id: 'sk_7', name: 'Node.js'},
        {id: 'sk_8', name: 'Python'},
        {id: 'sk_9', name: 'UI/UX Design'},
        {id: 'sk_10', name: 'DevOps'}
      ],
      totalCount: 48,
      pageNo: 1,
      pageSize: 20
    }).pipe(delay(300));
  }

  updateFreelancerProfile(formData: FormData): Observable<void> {
    console.log('Profile updated (mock)', formData);
    return of(void 0).pipe(delay(1200));
  }

  changePassword(request: any): Observable<void> {
    console.log('Password changed (mock)', request);
    return of(void 0).pipe(delay(900));
  }
}
