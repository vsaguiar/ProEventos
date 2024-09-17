import { Router } from '@angular/router';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/services/evento.service';
import { environment } from '@environments/environment';
import { PaginatedResult, Pagination } from '@app/models/Pagination';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss']
})
export class EventoListaComponent implements OnInit {

  modalRef?: BsModalRef;
  public eventos: Evento[] = [];
  public eventosFiltrados: Evento[] = [];
  public eventoId = 0;
  public pagination = {} as Pagination;

  public larguraImagem = 120;
  public margemImagem = 2;
  public mostrarImagem = true;
  private filtroListado = '';

  public get filtroLista(): string {
    return this.filtroListado;
  }

  public set filtroLista(value: string) {
    this.filtroListado = value;
    this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this.filtroLista) : this.eventos;
  }

  public filtrarEventos(filtrarPor: string): Evento[] {
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      (evento: any) => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1 ||
        evento.local.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    );
  }

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private router: Router
  ) { }

  public ngOnInit(): void {
    this.pagination = { currentPage: 1, itemsPerPage: 3, totalItems: 1 } as Pagination;
    this.carregarEventos();
  }

  public exibirImagem(): void {
    this.mostrarImagem = !this.mostrarImagem;
  }

  public mostraImagem(imagemURL: string): string {
    return (imagemURL != '')
      ? `${environment.apiURL}resources/images/${imagemURL}`
      : 'assets/semImagem.jpeg';
  }

  public carregarEventos(): void {
    this.spinnerShow();

    this.eventoService.getEventos(this.pagination.currentPage, this.pagination.itemsPerPage).subscribe({
      next: (paginatedResult: PaginatedResult<Evento[]>) => {
        this.eventos = paginatedResult.result;
        this.eventosFiltrados = this.eventos;
        this.pagination = paginatedResult.pagination;
      },
      error: (error: any) => {
        this.spinnerHide();
        this.toastr.error('Não foi possível carregar os eventos.', 'Erro!');
      },
      complete: () => this.spinnerHide()
    });
  }

  openModal(event: any, template: TemplateRef<any>, eventoId: number): void {
    event.stopPropagation();
    this.eventoId = eventoId;
    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  public pageChanged(event): void {
    this.pagination.currentPage = event.page;
    this.carregarEventos();
  }

  confirm(): void {
    this.modalRef?.hide();
    this.spinnerShow();

    this.eventoService.deleteEvento(this.eventoId).subscribe(
      (result: any) => {
        this.toastr.success('Evento deletado com sucesso.', 'Deletado!');
        this.carregarEventos()
      },
      (error: any) => {
        console.error(error);
        this.toastr.error(`Erro ao tentar deletar o evento ${this.eventoId}.`, 'Erro');
      }
    ).add(() => this.spinnerHide());
  }

  decline(): void {
    this.modalRef?.hide();
  }

  detalheEvento(id: number): void {
    this.router.navigate([`eventos/detalhe/${id}`]);
  }

  spinnerShow(): void {
    this.spinner.show();
  }

  spinnerHide(): void {
    this.spinner.hide();
  }

}
