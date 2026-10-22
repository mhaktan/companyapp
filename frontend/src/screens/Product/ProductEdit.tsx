import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkInput } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { useFlows } from '../../flows/FlowProvider';

type ProductRecord = {
  id: string | number;
  name: string;
  code?: string;
  unit?: string;
  unitPrice?: number;
};

interface ProductEditProps {
  record: ProductRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const ProductEdit: React.FC<ProductEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<ProductRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<ProductRecord>) =>
      dataProvider.update('Product', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'Product', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Edit Product</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Name *" value={String(form.name ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('name', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Code" value={String(form.code ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('code', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Unit" value={String(form.unit ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('unit', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Unit Price" value={String(form.unitPrice ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('unitPrice', v === '' ? null : Number(v)))(e.detail)} />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <TkButton label="Cancel" variant="secondary" onTkClick={onClose} />
            <TkButton label={mutation.isPending ? 'Saving…' : 'Save Changes'} variant="primary" mode="submit" disabled={mutation.isPending} />
          </div>
        </form>
      </div>
    </div>
  );
};
