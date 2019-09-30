import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})

// life cycle event that on init.
export class ValueComponent implements OnInit {

  values: any;

  // use this to inject a service for our components.
  constructor(private http: HttpClient) { }

  // on init lifecycle.
  ngOnInit() {
    this.getValues();
  }

  getValues() {

    // returns an observable which is a stream of data from our api. We need to subscribe to
    // the observable to get the data.
    this.http.get('http://localhost:5000/api/values').subscribe(response => {
      this.values = response;
    }, error => {
      console.log(error);
    });
  }

}
