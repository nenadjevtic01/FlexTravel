import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-stepper',
  templateUrl: './stepper.html',
  styleUrl: './stepper.scss',
})
export class Stepper {
  readonly value = input.required<number>();
  readonly min = input(0);
  readonly max = input(10);
  readonly label = input('');
  readonly valueChange = output<number>();
}
