import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  constructor(private http: HttpClient) { }

  // Function to get members
  getMembers(){
    // If the members array has data, return the data as an Observable
    if(this.members.length >0) return of(this.members);
  
    // If the members array is empty, make an HTTP GET request to fetch the data
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      // Use the map operator to transform the observable result
      map(members =>{
        // Store the fetched data in the members array
        this.members = members;
        // Return the fetched data
        return members;
      })
    )
  
  }

  getMember(username: string){
    const member = this.members.find(x=> x.userName ===username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username)

  }

  updateMember(member: Member) 
  {
    return this.http.put(this.baseUrl + 'users',member).pipe(
      map(() =>{
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member}
      })
    );
  }
 
}
