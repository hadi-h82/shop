import { DOCUMENT } from '@angular/common';
import { Injectable, inject } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';

export interface SeoData {
  title: string;
  description: string;
  canonicalUrl?: string;
  imageUrl?: string;
  type?: 'website' | 'product';
}

@Injectable({
  providedIn: 'root'
})
export class Seo {
  private readonly title = inject(Title);
  private readonly meta = inject(Meta);
  private readonly document = inject(DOCUMENT);

  update(data: SeoData): void {
    this.title.setTitle(data.title);

    this.meta.updateTag({
      name: 'description',
      content: data.description
    });

    this.meta.updateTag({
      property: 'og:title',
      content: data.title
    });

    this.meta.updateTag({
      property: 'og:description',
      content: data.description
    });

    this.meta.updateTag({
      property: 'og:type',
      content: data.type ?? 'website'
    });

    if (data.imageUrl) {
      this.meta.updateTag({
        property: 'og:image',
        content: data.imageUrl
      });
    }

    if (data.canonicalUrl) {
      this.meta.updateTag({
        property: 'og:url',
        content: data.canonicalUrl
      });

      this.setCanonicalUrl(data.canonicalUrl);
    }
  }

  private setCanonicalUrl(url: string): void {
    let link = this.document.head.querySelector<HTMLLinkElement>(
      'link[rel="canonical"]'
    );

    if (!link) {
      link = this.document.createElement('link');
      link.setAttribute('rel', 'canonical');
      this.document.head.appendChild(link);
    }

    link.setAttribute('href', url);
  }
}