import { apiFetch } from './http'

export interface Center {
  id: string
  code: string
  name: string
  region: string
  city: string
  address: string | null
  phone: string | null
  email: string | null
  status: string
  createdAtUtc: string
}

export interface GetCentersResponse {
  items: Center[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface GetCentersParams {
  page?: number
  pageSize?: number
  search?: string
  region?: string
  city?: string
  status?: string
}

export async function getCenters(
  params: GetCentersParams = {}
): Promise<GetCentersResponse> {
  const query = new URLSearchParams()

  query.set('page', String(params.page ?? 1))
  query.set('pageSize', String(params.pageSize ?? 10))

  if (params.search) {
    query.set('search', params.search)
  }

  if (params.region) {
    query.set('region', params.region)
  }

  if (params.city) {
    query.set('city', params.city)
  }

  if (params.status) {
    query.set('status', params.status)
  }

  return apiFetch<GetCentersResponse>(
    `/api/centers?${query.toString()}`
  )
}