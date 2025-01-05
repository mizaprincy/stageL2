export interface Employe {
  id_employe: string
  nom: string
  prenom: string
  email: string
  tel: string
  embauche: Date
  date_depart?: Date | null
  id_poste: number
  poste_Travail: PosteTravail | null
}

export interface PosteTravail {
  id_poste: number
  design: string
  salaire_horaire: number
  heures_travaillees: number
  statut: string
  description: string
  employes?: Employe[] | null
}

export interface User {
  userId: number
  nom: string
  prenom: string
  email: string
  role: string
  employeId?: string | null
  employe?: Employe | null
}

export interface DemandeAvance {
  demandeId: number
  employeId: number
  employe?: User | null
  montant: number
  nombreTranches: number
  dateDemande: Date
  statut: string
  message?: string | null
}

export interface Inbox {
  inboxId: number
  adminId?: number | null
  admin?: User | null
  employeId?: number | null
  employe?: User | null
  demandeId: number
  demandeAvance: DemandeAvance
  message: string
  dateNotification: Date
  isRead: boolean
}
