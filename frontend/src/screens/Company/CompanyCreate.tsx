import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkInput } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { useFlows } from '../../flows/FlowProvider';

type CompanyRecord = {
  id: string | number;
  name: string;
  taxNumber?: string;
  taxOffice?: string;
  address?: string;
  phone?: string;
  email?: string;
  contactPerson?: string;
};

interface CompanyCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const CompanyCreate: React.FC<CompanyCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<CompanyRecord>>({});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<CompanyRecord>) => dataProvider.create('Company', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'Company', values as Record<string, unknown>); onSuccess(); onClose(); setForm({}); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Create Company</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Name *" value={String(form.name ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('name', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Tax Number" value={String(form.taxNumber ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('taxNumber', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Address" value={String(form.address ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('address', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Tax Office" value={String(form.taxOffice ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('taxOffice', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Phone" value={String(form.phone ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('phone', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Email" value={String(form.email ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('email', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Contact Person" value={String(form.contactPerson ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('contactPerson', v))(e.detail)} />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <TkButton label="Cancel" variant="secondary" onTkClick={onClose} />
            <TkButton label={mutation.isPending ? 'Saving…' : 'Create'} variant="primary" mode="submit" disabled={mutation.isPending} />
          </div>
        </form>
      </div>
    </div>
  );
};
