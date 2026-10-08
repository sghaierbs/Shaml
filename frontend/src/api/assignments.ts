import { apiFetch } from './http'

export interface QueueAssignment {
    assignmentId: string
    caseId: string
    taskCode: string
    status: number
    createdAtUtc: string
}

export interface MyAssignment {
    assignmentId: string
    caseId: string
    taskCode: string
    status: number
    targetType: number
    createdAtUtc: string
}

export async function getRoleQueue(): Promise<QueueAssignment[]> {
    return apiFetch<QueueAssignment[]>(
        '/api/assignments/queue'
    )
}

export async function getMyAssignments(): Promise<MyAssignment[]> {
    return apiFetch<MyAssignment[]>(
        '/api/assignments/mine'
    )
}

export async function claimAssignment(
    assignmentId: string
): Promise<unknown> {
    return apiFetch(
        `/api/assignments/${assignmentId}/claim`,
        {
            method: 'POST',
        }
    )
}

export async function startAssignment(
    assignmentId: string
): Promise<unknown> {
    return apiFetch(
        `/api/assignments/${assignmentId}/start`,
        {
            method: 'POST',
        }
    )
}

export async function completeAssignment(
    assignmentId: string
): Promise<unknown> {
    return apiFetch(
        `/api/assignments/${assignmentId}/complete`,
        {
            method: 'POST',
        }
    )
}