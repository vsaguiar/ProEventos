import { Component, OnInit } from '@angular/core';
import { UserUpdate } from '@app/models/Identity/UserUpdate';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss']
})
export class PerfilComponent implements OnInit {
  
  public usuario = {} as UserUpdate;

  public get ehPalestrante(): boolean{
    return this.usuario.funcao == 'Palestrante';
  }

  constructor() { }

  get f(): any {
    return '';
  }

  public setFormValue(usuario: UserUpdate): void {
    this.usuario = usuario;
  }

  ngOnInit(): void {
  }

}
